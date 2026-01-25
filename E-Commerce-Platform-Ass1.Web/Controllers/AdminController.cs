using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Infrastructure.Extensions;
using E_Commerce_Platform_Ass1.Web.Models;
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
            var viewModel = dashboard.ToViewModel();
            return View(viewModel);
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
                return View(new List<AdminShopViewModel>());
            }

            var viewModels = result.Data!.ToViewModels();
            ViewBag.CurrentStatus = status;
            return View(viewModels);
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

            var viewModel = result.Data!.ToDetailViewModel();
            return View(viewModel);
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
                return View(new List<AdminProductViewModel>());
            }

            var viewModels = result.Data!.ToViewModels();
            return View(viewModels);
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
                return View(new List<AdminProductViewModel>());
            }

            var viewModels = result.Data!.ToViewModels();
            ViewBag.CurrentStatus = status;
            return View(viewModels);
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

            var viewModel = result.Data!.ToDetailViewModel();
            return View(viewModel);
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

        #region Category Management

        /// <summary>
        /// Danh sách tất cả Category
        /// </summary>
        public async Task<IActionResult> Categories()
        {
            var result = await _adminService.GetAllCategoriesAsync();
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                return View(new List<AdminCategoryViewModel>());
            }

            var viewModels = result.Data!.ToViewModels();
            return View(viewModels);
        }

        /// <summary>
        /// Form tạo Category mới
        /// </summary>
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View(new CreateCategoryViewModel());
        }

        /// <summary>
        /// Xử lý tạo Category mới
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dto = viewModel.ToDto();
            var result = await _adminService.CreateCategoryAsync(dto);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã tạo danh mục thành công!";
                return RedirectToAction(nameof(Categories));
            }

            TempData["Error"] = result.ErrorMessage;
            return View(viewModel);
        }

        /// <summary>
        /// Form sửa Category
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditCategory(Guid id)
        {
            var result = await _adminService.GetCategoryByIdAsync(id);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                return RedirectToAction(nameof(Categories));
            }

            var viewModel = result.Data!.ToEditViewModel();
            ViewBag.ProductCount = result.Data.ProductCount;
            return View(viewModel);
        }

        /// <summary>
        /// Xử lý sửa Category
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Guid id, EditCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dto = viewModel.ToDto();
            var result = await _adminService.UpdateCategoryAsync(id, dto);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Categories));
            }

            TempData["Error"] = result.ErrorMessage;
            return View(viewModel);
        }

        /// <summary>
        /// Xóa Category
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _adminService.DeleteCategoryAsync(id);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã xóa danh mục thành công!";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction(nameof(Categories));
        }

        #endregion
    }
}
