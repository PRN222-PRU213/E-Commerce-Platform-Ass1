using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Infrastructure.Extensions;
using E_Commerce_Platform_Ass1.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly IShopService _shopService;
        private readonly IProductService _productService;

        public ShopController(IShopService shopService, IProductService productService)
        {
            _shopService = shopService;
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var shop = await _shopService.GetShopDtoByIdAsync(id);
            if (shop == null)
            {
                return NotFound();
            }

            // Chỉ hiển thị shop đã được duyệt (Active)
            if (shop.Status != "Active")
            {
                return NotFound();
            }

            // Lấy danh sách sản phẩm của shop (chỉ sản phẩm đã được duyệt)
            var productsResult = await _productService.GetByShopIdAsync(id);
            var products =
                productsResult.IsSuccess && productsResult.Data != null
                    ? productsResult.Data.Where(p => p.Status == "active").ToList()
                    : new List<ProductDto>();

            var viewModel = new ShopDetailViewModel
            {
                Id = shop.Id,
                ShopName = shop.ShopName,
                Description = shop.Description,
                Status = shop.Status,
                CreatedAt = shop.CreatedAt,
                Products = products.ToShopProductViewModels(),
            };

            return View(viewModel);
        }

        [Authorize]
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
                TempData["ErrorMessage"] =
                    "Bạn đã có shop rồi. Mỗi tài khoản chỉ được đăng ký một shop.";
                return RedirectToAction("Profile", "Authentication");
            }

            return View();
        }

        [Authorize]
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
                ModelState.AddModelError(
                    string.Empty,
                    "Bạn đã có shop rồi. Mỗi tài khoản chỉ được đăng ký một shop."
                );
                return View(model);
            }

            // Kiểm tra tên shop đã tồn tại chưa
            var shopNameExists = await _shopService.ShopNameExistsAsync(model.ShopName.Trim());
            if (shopNameExists)
            {
                ModelState.AddModelError(
                    nameof(model.ShopName),
                    "Tên shop này đã được sử dụng. Vui lòng chọn tên khác."
                );
                return View(model);
            }

            // Tạo shop mới thông qua service layer
            await _shopService.RegisterShopAsync(userId, model.ShopName, model.Description);

            TempData["SuccessMessage"] =
                "Đăng ký shop thành công! Shop của bạn đang chờ phê duyệt.";
            return RedirectToAction("Profile", "Authentication");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewShop()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var shop = await _shopService.GetShopDtoByUserIdAsync(userId);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop. Vui lòng đăng ký shop trước.";
                return RedirectToAction("RegisterShop");
            }

            // Lấy toàn bộ sản phẩm của shop (tất cả status vì đây là trang quản lý)
            var productsResult = await _productService.GetByShopIdAsync(shop.Id);
            var products =
                productsResult.IsSuccess && productsResult.Data != null
                    ? productsResult.Data.ToList()
                    : new List<ProductDto>();

            var viewModel = new ShopDashboardViewModel
            {
                Id = shop.Id,
                UserId = shop.UserId,
                ShopName = shop.ShopName,
                Description = shop.Description,
                Status = shop.Status,
                CreatedAt = shop.CreatedAt,
                Products = products.ToShopProductViewModels(),
            };

            return View(viewModel);
        }

        /// <summary>
        /// Trang thống kê doanh thu của Shop
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var shop = await _shopService.GetShopDtoByUserIdAsync(userId);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop. Vui lòng đăng ký shop trước.";
                return RedirectToAction("RegisterShop");
            }

            // Kiểm tra shop đã được duyệt chưa
            if (shop.Status != "Active")
            {
                TempData["ErrorMessage"] =
                    "Shop của bạn chưa được duyệt. Vui lòng chờ admin phê duyệt.";
                return RedirectToAction("ViewShop");
            }

            var statistics = await _shopService.GetShopStatisticsAsync(shop.Id);
            var viewModel = statistics.ToViewModel();
            return View(viewModel);
        }
    }
}
