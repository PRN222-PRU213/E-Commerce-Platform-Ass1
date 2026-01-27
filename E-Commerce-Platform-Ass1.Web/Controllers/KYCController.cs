using System;
using System.Security.Claims;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    [Authorize]
    public class KYCController : Controller
    {
        private readonly IEkycService _eKycService;

        public KYCController(IEkycService eKycService)
        {
            _eKycService = eKycService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TempData.Remove("ErrorMessage");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var isVerified = await _eKycService.IsUserVerifiedAsync(userId);
            if (isVerified)
            {
                return RedirectToAction("Status");
            }

            return RedirectToAction("Verify");
        }

        [HttpGet]
        public async Task<IActionResult> Verify()
        {
            TempData.Remove("ErrorMessage");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var isVerified = await _eKycService.IsUserVerifiedAsync(userId);
            if (isVerified)
            {
                return RedirectToAction("Status");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(VerifyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            try
            {
                var result = await _eKycService.VerifyAndSaveAsync(userId, model.FrontCard, model.BackCard, model.Selfie);
                if (result.IsSuccess)
                {
                    TempData.Remove("ErrorMessage");
                    TempData["SuccessMessage"] = "Xác thực danh tính thành công!";
                    return RedirectToAction("Status");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message ?? "Xác thực danh tính thất bại. Vui lòng kiểm tra lại ảnh chụp và thử lại.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình xác thực: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Status()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var isVerified = await _eKycService.IsUserVerifiedAsync(userId);
            
            var viewModel = new KYCStatusViewModel
            {
                IsVerified = isVerified,
                Status = isVerified ? "VERIFIED" : "NOT_VERIFIED",
                Message = isVerified ? "Tài khoản của bạn đã được xác thực danh tính." : "Tài khoản của bạn chưa được xác thực danh tính."
            };

            return View(viewModel);
        }
    }
}
