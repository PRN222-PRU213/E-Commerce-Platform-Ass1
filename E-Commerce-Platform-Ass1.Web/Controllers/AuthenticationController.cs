using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserService _userService;

        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Server-side validation
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Trim and normalize email
            model.Email = model.Email.Trim().ToLowerInvariant();

            // Check if email is verified first
            var isVerified = await _userService.IsEmailVerifiedAsync(model.Email);
            if (!isVerified)
            {
                // Check if user exists (email might not exist at all)
                var user = await _userService.ValidateUserAsync(model.Email, model.Password);
                if (user == null)
                {
                    // Could be wrong password OR unverified email
                    // For security, show generic message but also check verification
                    ModelState.AddModelError(string.Empty, "Email chưa được xác thực hoặc thông tin đăng nhập không đúng.");
                    ViewData["ShowResendLink"] = true;
                    ViewData["Email"] = model.Email;
                    return View(model);
                }
            }

            var validatedUser = await _userService.ValidateUserAsync(model.Email, model.Password);
            if (validatedUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, validatedUser.Id.ToString()),
                new Claim(ClaimTypes.Name, validatedUser.Name),
                new Claim(ClaimTypes.Email, validatedUser.Email),
                new Claim(ClaimTypes.Role, validatedUser.Role),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = model.RememberMe }
            );

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Redirect Admin to Admin Dashboard
            if (validatedUser.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Server-side validation
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Trim whitespace from inputs
            model.Name = model.Name.Trim();
            model.Email = model.Email.Trim().ToLowerInvariant();

            // Get base URL for verification link
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            // Register with email verification
            var result = await _userService.RegisterWithVerificationAsync(model.Name, model.Email, model.Password, baseUrl);
            
            if (!result.Success)
            {
                ModelState.AddModelError(nameof(model.Email), result.ErrorMessage ?? "Đã có lỗi xảy ra.");
                return View(model);
            }

            // Redirect to verification pending page
            TempData["Email"] = model.Email;
            TempData["EmailSent"] = result.EmailSent;
            return RedirectToAction("VerificationPending");
        }

        [HttpGet]
        public IActionResult VerificationPending()
        {
            var email = TempData["Email"]?.ToString();
            var emailSent = TempData["EmailSent"] as bool? ?? true;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Register");
            }

            ViewData["Email"] = email;
            ViewData["EmailSent"] = emailSent;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                ViewData["Success"] = false;
                ViewData["ErrorMessage"] = "Link xác thực không hợp lệ.";
                return View();
            }

            var result = await _userService.VerifyEmailAsync(token);

            ViewData["Success"] = result.Success;
            ViewData["UserName"] = result.UserName;
            ViewData["ErrorMessage"] = result.ErrorMessage;

            return View();
        }

        [HttpGet]
        public IActionResult ResendVerification()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendVerification(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("email", "Vui lòng nhập email.");
                return View();
            }

            email = email.Trim().ToLowerInvariant();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var result = await _userService.ResendVerificationEmailAsync(email, baseUrl);

            if (!result.Success)
            {
                ModelState.AddModelError("email", result.ErrorMessage ?? "Đã có lỗi xảy ra.");
                return View();
            }

            TempData["SuccessMessage"] = "Email xác thực đã được gửi! Vui lòng kiểm tra hộp thư của bạn.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
