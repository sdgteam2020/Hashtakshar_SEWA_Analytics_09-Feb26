using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Domain.Identity;
using HastaksharSewaAnalytics.Infrastructure.Persistence;
using HastaksharSewaAnalytics.Infrastructure.Repository.Common;
using HastaksharSewaAnalytics.Infrastructure.Security;
using HastaksharSewaAnalytics.Infrastructure.Services;
using HastaksharSewaAnalytics.Presentation.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddControllersWithViews();
 
builder.Services.AddDbContext<HastaksharSewaAnalyticsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
 
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IClientLogsService, ClientLogsService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDigitalSignService, DigitalSignService>();
builder.Services.AddScoped<IDeviceKeyHasher, DeviceKeyHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
 
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<HastaksharSewaAnalyticsDbContext>()
    .AddDefaultTokenProviders();
 
builder.Services.Configure<SecurityStampValidatorOptions>(o =>
{
    o.ValidationInterval = TimeSpan.Zero;
});
 
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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.SlidingExpiration = true;

    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/Login";

    options.Events = new CookieAuthenticationEvents
    { 
        OnValidatePrincipal = async context =>
        {
            var userManager = context.HttpContext.RequestServices
                .GetRequiredService<UserManager<ApplicationUser>>();

            var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            var claimSessionId = context.Principal?.FindFirstValue("active_session_id");

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(claimSessionId))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                return;
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrWhiteSpace(user.ActiveSessionId) || user.ActiveSessionId != claimSessionId)
            { 
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            }
        },
         
        OnRedirectToLogin = ctx =>
        {
            if (ctx.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return ctx.Response.WriteAsJsonAsync(new { message = "Unauthorized. Please login again." });
            }

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

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.HeaderName = "RequestVerificationToken";
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontEnd", policy =>
    {
        policy.WithOrigins("https://localhost:7018") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
 
ErrorLog.Env = builder.Environment;
var app = builder.Build();
 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["Content-Security-Policy"] = "frame-ancestors 'none';";
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontEnd");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
