using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;
using HastaksharSewaAnalytics.Application.Dtos.User;
using HastaksharSewaAnalytics.Infrastructure.Identity;
using HastaksharSewaAnalytics.Presentation.Helpers;
using HastaksharSewaAnalytics.Presentation.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace HastaksharSewaAnalytics.Presentation.Controllers;

public sealed class AuthController : Controller
{
    public const string SessionKeySalt = "_Salt";
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenService jwtTokenService, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    [HttpGet, AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {

        ViewData["ReturnUrl"] = returnUrl;
        string dd = AESEncrytDecry.GetSalt();
        HttpContext.Session.SetString(SessionKeySalt, dd);
        ViewBag.hdns= dd;
        return View();
    }

    [HttpPost, AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                      || (Request.Headers["Accept"].ToString()?.Contains("application/json") ?? false);

        try
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                var msg = "Username and password are required.";
                if (isAjax) return BadRequest(new { message = msg });
                TempData["ToastError"] = msg;
                return View();
            }

            var salt = HttpContext.Session.GetString(SessionKeySalt);
            if (string.IsNullOrWhiteSpace(salt))
            {
                var msg = "Session expired. Please refresh the page and try again.";
                if (isAjax) return BadRequest(new { message = msg });
                TempData["ToastError"] = msg;
                return View();
            }

            username = AESEncrytDecry.DecryptAES(username.Trim(), salt);
            password = AESEncrytDecry.DecryptAES(password.Trim(), salt);

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9._@-]+$"))
            {
                var msg = "Username contains invalid characters.";
                if (isAjax) return BadRequest(new { message = msg });
                TempData["ToastError"] = msg;
                return View();
            }


            var user = await _userManager.FindByNameAsync(username)
                       ?? await _userManager.FindByEmailAsync(username);

            if (user is null)
            {
                var msg = "Invalid username/email or password.";
                if (isAjax) return BadRequest(new { message = msg });
                TempData["ToastError"] = msg;
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                password,
                isPersistent: false,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
                new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new("token_type", "user"),
                new("scope", "analytics.read analytics.write")
            };

                var expiresUtc = DateTime.UtcNow.AddMinutes(_jwtTokenService.AccessTokenMinutes);
                var jwt = _jwtTokenService.CreateToken(claims, expiresUtc);

                var redirect = (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    ? returnUrl
                    : Url.Action("Dashboard", "Dashboard");

                if (isAjax)
                {
                    return Ok(new
                    {
                        redirectUrl = redirect,
                        message = "Login successful.",
                        accessToken = jwt,
                        expiresInSeconds = _jwtTokenService.AccessTokenMinutes * 60,
                        tokenType = "Bearer"
                    });
                }

                TempData["ToastSuccess"] = "Login successful.";
                return Redirect(redirect);
            }

            var error = result.IsLockedOut
                ? "Account locked due to multiple failed attempts. Try again later."
                : "Invalid username/email or password.";

            if (isAjax) return BadRequest(new { message = error });
            TempData["ToastError"] = error;
            return View();
        }
        catch (CryptographicException ex)
        {
            _logger.LogError(ex, "Crypto error during login.");
            ErrorLog.LogErrorToFile(ex, "Crypto error in AuthController.Login");
            var msg = "Security error occurred. Please refresh the page and try again.";
            if (isAjax) return StatusCode(500, new { message = msg });
            TempData["ToastError"] = msg;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login.");
            ErrorLog.LogErrorToFile(ex, "Unexpected error in AuthController.Login");
            var msg = "Unexpected server error. Please try again later.";
            if (isAjax) return StatusCode(500, new { message = msg });
            TempData["ToastError"] = msg;
            return View();
        }
    }


    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
                await _userManager.UpdateSecurityStampAsync(user);

            // 🔐 Hard sign-out (server + cookie scheme)
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            await _signInManager.SignOutAsync(); // keeps Identity state clean

            HttpContext.Session.Clear();

            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            Response.Cookies.Delete(".AspNetCore.Session");

            TempData["ToastInfo"] = "Logged out successfully.";
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            // log the real error
            ErrorLog.LogErrorToFile(ex, "Unexpected error in AuthController.Logout");

            // try best-effort cleanup even if something failed above
            try
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete(".AspNetCore.Identity.Application");
                Response.Cookies.Delete(".AspNetCore.Session");
            }
            catch { /* ignore */ }

            TempData["ToastError"] = "Logout failed due to a server error. Please try again.";
            return RedirectToAction("Login", "Auth");
        }
    }


    [HttpGet, AllowAnonymous]
    public IActionResult Register() => View(new RegisterVm());

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVm model)
    {
        try
        {
            if (!ModelState.IsValid) return View(model);

            if (!Regex.IsMatch(model.Username ?? "", @"^[a-zA-Z0-9._-]+$"))
            {
                ModelState.AddModelError("Username", "Username contains invalid characters.");
                return View(model);
            }


            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);

                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            TempData["ToastSuccess"] = "Registration successful.";
            return RedirectToAction("Dashboard", "Dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration.");
            ErrorLog.LogErrorToFile(ex, "Unexpected error in AuthController.Register");
            TempData["ToastError"] = "Registration failed due to server error. Please try again later.";
            return View(model);
        }
    }

}
