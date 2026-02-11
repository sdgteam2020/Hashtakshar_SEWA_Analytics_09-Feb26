using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Infrastructure.Identity;
using HastaksharSewaAnalytics.Infrastructure.Persistence;
using HastaksharSewaAnalytics.Infrastructure.Repository.Common;
using HastaksharSewaAnalytics.Infrastructure.Security;
using HastaksharSewaAnalytics.Infrastructure.Services;
using HastaksharSewaAnalytics.Presentation.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ================== MVC ==================
builder.Services.AddControllersWithViews();

// ================== DB ===================
builder.Services.AddDbContext<HastaksharSewaAnalyticsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// ================== DI ===================
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IClientLogsService, ClientLogsService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDigitalSignService, DigitalSignService>();
builder.Services.AddScoped<IDeviceKeyHasher, DeviceKeyHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();

// ================== Identity ==================
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<HastaksharSewaAnalyticsDbContext>()
    .AddDefaultTokenProviders();

// ✅ Invalidate cookies immediately after logout
builder.Services.Configure<SecurityStampValidatorOptions>(o =>
{
    o.ValidationInterval = TimeSpan.Zero;
});

// ================== Auth Schemes ==================
var key = builder.Configuration["Jwt:Key"]!;
var issuer = builder.Configuration["Jwt:Issuer"]!;
var audience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
.AddJwtBearer("DeviceBearer", o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

// ================== Authorization Policies ==================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeviceOnly", p =>
        p.AddAuthenticationSchemes("DeviceBearer")
         .RequireAuthenticatedUser()
         .RequireClaim("token_type", "device"));

    options.AddPolicy("UserOnly", p =>
        p.RequireAuthenticatedUser()
         .RequireClaim("token_type", "user"));

    options.AddPolicy("DeviceOrUser", p =>
        p.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, "DeviceBearer")
         .RequireAuthenticatedUser()
         .RequireClaim("token_type", "device", "user"));
});

// ================== Cookie Settings ==================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.SlidingExpiration = true;

    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/Login";

    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = ctx =>
        {
            // AJAX/API -> return 401 JSON
            if (ctx.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return ctx.Response.WriteAsJsonAsync(new { message = "Unauthorized. Please login again." });
            }

            // Browser -> redirect with reason flag
            var uri = QueryHelpers.AddQueryString(ctx.RedirectUri, "reason", "auth");
            ctx.Response.Redirect(uri);
            return Task.CompletedTask;
        },

        OnRedirectToAccessDenied = ctx =>
        {
            var uri = QueryHelpers.AddQueryString(ctx.RedirectUri, "reason", "denied");
            ctx.Response.Redirect(uri);
            return Task.CompletedTask;
        }
    };
});

// ================== Antiforgery ==================
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.HeaderName = "RequestVerificationToken";
});

// ================== Session ==================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// ================== CORS ==================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontEnd", policy =>
    {
        policy.WithOrigins("https://localhost:7018") // change for prod
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
// 👇 Env here
ErrorLog.Env = builder.Environment;
var app = builder.Build();

// ================== Pipeline ==================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontEnd");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// (Optional) block obvious curl bots (NOT a security control)
//app.Use(async (ctx, next) =>
//{
//    var ua = ctx.Request.Headers["User-Agent"].ToString();
//    if (string.IsNullOrWhiteSpace(ua) || ua.Contains("curl", StringComparison.OrdinalIgnoreCase))
//    {
//        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
//        await ctx.Response.WriteAsync("Forbidden");
//        return;
//    }
//    await next();
//});

//Console.WriteLine("ContentRootPath: " + ErrorLog.Env.ContentRootPath);
//Console.WriteLine("BaseDirectory: " + AppContext.BaseDirectory);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
