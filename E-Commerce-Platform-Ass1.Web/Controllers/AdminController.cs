using E_Commerce_Platform_Ass1.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    /// <summary>
    /// Controller xử lý các chức năng Admin
    /// - Quản lý Shop
    /// - Phê duyệt sản phẩm
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        #region Dashboard

        /// <summary>
        /// Trang Dashboard của Admin
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var dashboard = await _adminService.GetDashboardStatisticsAsync();
            return View(dashboard);
        }

        #endregion

        #region Shop Management

        /// <summary>
        /// Danh sách tất cả Shop
        /// </summary>
        public async Task<IActionResult> Shops(string? status = null)
        {
            var result = string.IsNullOrEmpty(status)
                ? await _adminService.GetAllShopsAsync()
                : await _adminService.GetShopsByStatusAsync(status);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                return View(new List<E_Commerce_Platform_Ass1.Service.DTOs.ShopDto>());
            }

            ViewBag.CurrentStatus = status;
            return View(result.Data);
        }

        /// <summary>
        /// Chi tiết Shop
        /// </summary>
        public async Task<IActionResult> ShopDetail(Guid id)
        {
            var result = await _adminService.GetShopDetailAsync(id);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                return RedirectToAction(nameof(Shops));
            }

            return View(result.Data);
        }

        /// <summary>
        /// Duyệt Shop
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ApproveShop(Guid id)
        {
            var result = await _adminService.ApproveShopAsync(id);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Shop đã được duyệt thành công!";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction(nameof(ShopDetail), new { id });
        }

        /// <summary>
        /// Từ chối Shop
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RejectShop(Guid id, string? reason)
        {
            var result = await _adminService.RejectShopAsync(id, reason);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Shop đã bị từ chối!";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction(nameof(ShopDetail), new { id });
        }

        #endregion

        #region Product Approval

        /// <summary>
        /// Danh sách sản phẩm chờ duyệt
        /// </summary>
        public async Task<IActionResult> PendingProducts()
        {
            var result = await _adminService.GetPendingProductsAsync();
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                return View(new List<E_Commerce_Platform_Ass1.Service.DTOs.ProductDto>());
            }

            return View(result.Data);
        }

        /// <summary>
        /// Danh sách tất cả sản phẩm
        /// </summary>
        public async Task<IActionResult> AllProducts(string? status = null)
        {
            var result = string.IsNullOrEmpty(status)
                ? await _adminService.GetAllProductsAsync()
                : await _adminService.GetProductsByStatusAsync(status);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                return View(new List<E_Commerce_Platform_Ass1.Service.DTOs.ProductDto>());
            }

            ViewBag.CurrentStatus = status;
            return View(result.Data);
        }

        /// <summary>
        /// Chi tiết sản phẩm cần duyệt
        /// </summary>
        public async Task<IActionResult> ProductDetail(Guid id)
        {
            var result = await _adminService.GetProductForApprovalAsync(id);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                return RedirectToAction(nameof(PendingProducts));
            }

            return View(result.Data);
        }

        /// <summary>
        /// Duyệt sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ApproveProduct(Guid id)
        {
            var result = await _adminService.ApproveProductAsync(id);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Sản phẩm đã được duyệt thành công!";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction(nameof(PendingProducts));
        }

        /// <summary>
        /// Từ chối sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RejectProduct(Guid id, string? reason)
        {
            var result = await _adminService.RejectProductAsync(id, reason);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Sản phẩm đã bị từ chối!";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction(nameof(PendingProducts));
        }

        #endregion
    }
}
