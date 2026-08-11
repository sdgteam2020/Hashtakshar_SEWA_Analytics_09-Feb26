using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;
using HastaksharSewaAnalytics.Application.Dtos.User;
using HastaksharSewaAnalytics.Domain.Identity;
using HastaksharSewaAnalytics.Presentation.Helpers;
using HastaksharSewaAnalytics.Presentation.Logging;
using HastaksharSewaAnalytics.Presentation.Models;
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
    public const string ActiveSessionClaim = "active_session_id";


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
        string dd = AesEncrytDecry.GetSalt();
        HttpContext.Session.SetString(SessionKeySalt, dd);
        ViewBag.hdns = dd;
        return View();
    }

    [HttpPost, AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Consumes("multipart/form-data", "application/x-www-form-urlencoded")]
    public async Task<IActionResult> Login([FromForm] LoginRequest req, CancellationToken ct)
    {
        bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                      || (Request.Headers["Accept"].ToString()?.Contains("application/json") ?? false);

        try
        {
            if (Request.Query.ContainsKey("username") || Request.Query.ContainsKey("password") ||
                Request.Query.ContainsKey("Username") || Request.Query.ContainsKey("Password"))
            {
                var msg = "Do not send credentials in query string.";
                if (isAjax) return BadRequest(new { message = msg });
                TempData["ToastError"] = msg;
                return View();
            }

            var username = req.Username;
            var password = req.Password;
            var returnUrl = req.ReturnUrl;

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

            username = AesEncrytDecry.DecryptAES(username.Trim(), salt);
            password = AesEncrytDecry.DecryptAES(password.Trim(), salt);

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
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
                var msg = "Invalid username or password.";
                if (isAjax) return BadRequest(new { message = msg });
                TempData["ToastError"] = msg;
                return View();
            }


            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            await _signInManager.SignOutAsync();

            HttpContext.Session.Clear();

            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            Response.Cookies.Delete(".AspNetCore.Session");


            await _userManager.UpdateSecurityStampAsync(user);

            var result = await _signInManager.PasswordSignInAsync(
                user,
                password,
                isPersistent: false,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                var newSessionId = Guid.NewGuid().ToString("N");
                user.ActiveSessionId = newSessionId;
                user.ActiveSessionIssuedUtc = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                await _signInManager.SignOutAsync();
                await _signInManager.SignInWithClaimsAsync(
                    user,
                    isPersistent: false,
                    additionalClaims: new[] { new Claim(ActiveSessionClaim, newSessionId) }
                );

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
                return Redirect(redirect!);
            }

            var error = result.IsLockedOut
                ? "Account locked due to multiple failed attempts. Try again later."
                : "Invalid username or password.";

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
            {
                user.ActiveSessionId = null;
                user.ActiveSessionIssuedUtc = null;
                await _userManager.UpdateAsync(user);

                await _userManager.UpdateSecurityStampAsync(user);
            }

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            await _signInManager.SignOutAsync();

            HttpContext.Session.Clear();

            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            Response.Cookies.Delete(".AspNetCore.Session");

            TempData["ToastInfo"] = "Logged out successfully.";
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "Unexpected error in AuthController.Logout");

            try
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete(".AspNetCore.Identity.Application");
                Response.Cookies.Delete(".AspNetCore.Session");
            }
            catch { }

            TempData["ToastError"] = "Logout failed due to a server error. Please try again.";
            return RedirectToAction("Login", "Auth");
        }
    }



    [HttpGet, AllowAnonymous]
    public IActionResult Register()
    {
        string dd = AesEncrytDecry.GetSalt();
        HttpContext.Session.SetString(SessionKeySalt, dd);
        ViewBag.hdns = dd;
        return View(new RegisterVm());
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    [Consumes("multipart/form-data", "application/x-www-form-urlencoded")]
    public async Task<IActionResult> Register([FromForm] RegisterEncryptedVm encryptedModel)
    {
        bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                      || (Request.Headers["Accept"].ToString()?.Contains("application/json") ?? false);

        try
        {
            if (Request.Query.ContainsKey("username") || Request.Query.ContainsKey("password") ||
                Request.Query.ContainsKey("confirmPassword") || Request.Query.ContainsKey("Username") ||
                Request.Query.ContainsKey("Password") || Request.Query.ContainsKey("ConfirmPassword"))
            {
                var msg = "Do not send credentials in query string.";
                if (isAjax) return BadRequest(new { message = msg });
                TempData["ToastError"] = msg;
                return View();
            }

            if (string.IsNullOrWhiteSpace(encryptedModel.Username) ||
                string.IsNullOrWhiteSpace(encryptedModel.Password) ||
                string.IsNullOrWhiteSpace(encryptedModel.ConfirmPassword))
            {
                var msg = "Username, Password and Confirm Password are required.";
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

            var model = new RegisterVm
            {
                Username = AesEncrytDecry.DecryptAES(encryptedModel.Username.Trim(), salt),
                Password = AesEncrytDecry.DecryptAES(encryptedModel.Password.Trim(), salt),
                ConfirmPassword = AesEncrytDecry.DecryptAES(encryptedModel.ConfirmPassword.Trim(), salt)
            };

            if (!Regex.IsMatch(model.Username ?? "", @"^[a-zA-Z0-9._-]+$"))
            {
                var msg = "Username contains invalid characters.";
                if (isAjax) return BadRequest(new { message = msg });
                ModelState.AddModelError("Username", msg);
                return View();
            }

            if (model.Password != model.ConfirmPassword)
            {
                var msg = "Password and Confirm Password do not match.";
                if (isAjax) return BadRequest(new { message = msg });
                ModelState.AddModelError("ConfirmPassword", msg);
                return View();
            }
             
            ModelState.Clear();
            if (!TryValidateModel(model))
            {
                if (isAjax)
                {
                    var allErrors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList();

                    return BadRequest(new
                    {
                        message = allErrors.FirstOrDefault() ?? "Validation failed."
                    });
                }

                return View();
            }

            var user = new ApplicationUser
            {
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();

                foreach (var err in errors)
                    ModelState.AddModelError("", err);

                if (isAjax)
                    return BadRequest(new { message = errors.FirstOrDefault() ?? "Registration failed." });

                return View();
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            var redirect = Url.Action("Dashboard", "Dashboard");

            if (isAjax)
            {
                return Ok(new
                {
                    redirectUrl = redirect,
                    message = "Registration successful."
                });
            }

            TempData["ToastSuccess"] = "Registration successful.";
            return RedirectToAction("Dashboard", "Dashboard");
        }
        catch (CryptographicException ex)
        {
            _logger.LogError(ex, "Crypto error during registration.");
            ErrorLog.LogErrorToFile(ex, "Crypto error in AuthController.Register");

            var msg = "Security error occurred. Please refresh the page and try again.";
            if (isAjax) return StatusCode(500, new { message = msg });

            TempData["ToastError"] = msg;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration.");
            ErrorLog.LogErrorToFile(ex, "Unexpected error in AuthController.Register");

            var msg = "Registration failed due to server error. Please try again later.";
            if (isAjax) return StatusCode(500, new { message = msg });

            TempData["ToastError"] = msg;
            return View();
        }
    }

    [Authorize]
    [HttpGet]
    public IActionResult PingAuth() => Ok(new { ok = true });
}
