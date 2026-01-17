using System.Security.Claims;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    /// <summary>
    /// Controller quản lý sản phẩm cho Shop
    /// </summary>
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShopRepository _shopRepository;

        public ProductsController(IProductService productService, IShopRepository shopRepository)
        {
            _productService = productService;
            _shopRepository = shopRepository;
        }

        /// <summary>
        /// Lấy UserId từ Claims
        /// </summary>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Guid.Empty;
            }
            return userId;
        }

        /// <summary>
        /// Lấy Shop của user hiện tại
        /// </summary>
        private async Task<Data.Database.Entities.Shop?> GetCurrentUserShopAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return null;
            // return await _shopRepository.GetByUserIdAsync(userId);
            return null;
        }

        /// <summary>
        /// GET /Products - Danh sách sản phẩm của shop
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var shop = await GetCurrentUserShopAsync();
            if (shop == null)
            {
                TempData["ErrorMessage"] =
                    "Bạn chưa có shop. Vui lòng đăng ký shop trước khi quản lý sản phẩm.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _productService.GetByShopIdAsync(shop.Id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new ProductListViewModel
            {
                ShopId = shop.Id,
                ShopName = shop.ShopName,
                Products =
                    result
                        .Data?.Select(p => new ProductListItemViewModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            BasePrice = p.BasePrice,
                            Status = p.Status,
                            ImageUrl = p.ImageUrl,
                            CreatedAt = p.CreatedAt,
                            CategoryName = p.CategoryName,
                        })
                        .ToList() ?? new List<ProductListItemViewModel>(),
            };

            return View(viewModel);
        }

        /// <summary>
        /// GET /Products/Create - Form tạo sản phẩm
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var shop = await GetCurrentUserShopAsync();
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop. Vui lòng đăng ký shop trước.";
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new CreateProductViewModel();
            await PopulateCategoriesAsync(viewModel);

            return View(viewModel);
        }

        /// <summary>
        /// POST /Products/Create - Xử lý tạo sản phẩm
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel viewModel)
        {
            var shop = await GetCurrentUserShopAsync();
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync(viewModel);
                return View(viewModel);
            }

            var dto = new CreateProductDto
            {
                ShopId = shop.Id,
                CategoryId = viewModel.CategoryId,
                Name = viewModel.Name,
                Description = viewModel.Description ?? string.Empty,
                BasePrice = viewModel.BasePrice,
                ImageUrl = viewModel.ImageUrl ?? string.Empty,
            };

            var result = await _productService.CreateProductAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.ErrorMessage ?? "Không thể tạo sản phẩm."
                );
                await PopulateCategoriesAsync(viewModel);
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Tạo sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Load danh sách categories vào dropdown
        /// </summary>
        private async Task PopulateCategoriesAsync(CreateProductViewModel viewModel)
        {
            var categories = await _productService.GetAllCategoriesAsync();
            viewModel.Categories = categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
        }
    }
}
