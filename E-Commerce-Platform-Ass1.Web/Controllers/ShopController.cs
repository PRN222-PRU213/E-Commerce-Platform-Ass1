using System;
using System.Security.Claims;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }

        [HttpGet]
        public async Task<IActionResult> RegisterShop()
        {
            // Kiểm tra xem user đã có shop chưa
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var hasShop = await _shopService.UserHasShopAsync(userId);
            if (hasShop)
            {
                TempData["ErrorMessage"] = "Bạn đã có shop rồi. Mỗi tài khoản chỉ được đăng ký một shop.";
                return RedirectToAction("Profile", "Authentication");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterShop(RegisterShopViewModel model)
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

            // Kiểm tra xem user đã có shop chưa
            var hasShop = await _shopService.UserHasShopAsync(userId);
            if (hasShop)
            {
                ModelState.AddModelError(string.Empty, "Bạn đã có shop rồi. Mỗi tài khoản chỉ được đăng ký một shop.");
                return View(model);
            }

            // Kiểm tra tên shop đã tồn tại chưa
            var shopNameExists = await _shopService.ShopNameExistsAsync(model.ShopName.Trim());
            if (shopNameExists)
            {
                ModelState.AddModelError(nameof(model.ShopName), "Tên shop này đã được sử dụng. Vui lòng chọn tên khác.");
                return View(model);
            }

            // Tạo shop mới thông qua service layer
            await _shopService.RegisterShopAsync(userId, model.ShopName, model.Description);

            TempData["SuccessMessage"] = "Đăng ký shop thành công! Shop của bạn đang chờ phê duyệt.";
            return RedirectToAction("Profile", "Authentication");
        }

        [HttpGet]
        public async Task<IActionResult> ViewShop()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var shop = await _shopService.GetShopByUserIdAsync(userId);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop. Vui lòng đăng ký shop trước.";
                return RedirectToAction("RegisterShop");
            }

            return View(shop);
        }
    }
}
