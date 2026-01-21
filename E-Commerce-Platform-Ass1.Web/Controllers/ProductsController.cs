using System.Security.Claims;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.DTOs;
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
        private readonly IProductVariantService _productVariantService;
        private readonly IShopService _shopService;

        public ProductsController(
            IProductService productService,
            IProductVariantService productVariantService,
            IShopService shopService
        )
        {
            _productService = productService;
            _productVariantService = productVariantService;
            _shopService = shopService;
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
            return await _shopService.GetShopByUserIdAsync(userId);
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

            return View("~/Views/Shop/Products/Index.cshtml", viewModel);
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

            return View("~/Views/Shop/Products/Create.cshtml", viewModel);
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
                return View("~/Views/Shop/Products/Create.cshtml", viewModel);
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
                return View("~/Views/Shop/Products/Create.cshtml", viewModel);
            }

            TempData["SuccessMessage"] =
                "Tạo sản phẩm thành công! Vui lòng thêm biến thể cho sản phẩm.";
            return RedirectToAction("Edit", new { id = result.Data });
        }

        /// <summary>
        /// GET /Products/Detail/{id} - Xem chi tiết sản phẩm (readonly)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var shop = await GetCurrentUserShopAsync();
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _productService.GetProductDetailAsync(id, shop.Id);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index");
            }

            var product = result.Data;
            var viewModel = new ProductDetailViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                ImageUrl = product.ImageUrl,
                Status = product.Status,
                CategoryName = product.CategoryName,
                AvgRating = product.AvgRating,
                CreatedAt = product.CreatedAt,
                Variants = product.Variants ?? new List<ProductVariantDto>(),
            };

            return View(viewModel);
        }

        /// <summary>
        /// GET /Products/Edit/{id} - Trang chỉnh sửa sản phẩm và quản lý variants
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var shop = await GetCurrentUserShopAsync();
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _productService.GetProductDetailAsync(id, shop.Id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Index");
            }

            var product = result.Data!;
            var categories = await _productService.GetAllCategoriesAsync();

            var viewModel = new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                Status = product.Status,
                ImageUrl = product.ImageUrl,
                CategoryName = product.CategoryName,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt,
                Categories = categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name,
                        Selected = c.Id == product.CategoryId,
                    })
                    .ToList(),
                Variants = product
                    .Variants.Select(v => new ProductVariantViewModel
                    {
                        Id = v.Id,
                        VariantName = v.VariantName,
                        Price = v.Price,
                        Size = v.Size,
                        Color = v.Color,
                        Stock = v.Stock,
                        Sku = v.Sku,
                        Status = v.Status,
                        ImageUrl = v.ImageUrl,
                    })
                    .ToList(),
            };

            return View("~/Views/Shop/Products/Edit.cshtml", viewModel);
        }

        /// <summary>
        /// POST /Products/Update/{id} - Cập nhật thông tin sản phẩm
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id, EditProductViewModel viewModel)
        {
            var shop = await GetCurrentUserShopAsync();
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index", "Home");
            }

            // Validate model
            if (!ModelState.IsValid)
            {
                // Reload categories and variants
                var categories = await _productService.GetAllCategoriesAsync();
                viewModel.Categories = categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name,
                        Selected = c.Id == viewModel.CategoryId,
                    })
                    .ToList();

                var detailResult = await _productService.GetProductDetailAsync(id, shop.Id);
                if (detailResult.IsSuccess)
                {
                    viewModel.Variants = detailResult
                        .Data!.Variants.Select(v => new ProductVariantViewModel
                        {
                            Id = v.Id,
                            VariantName = v.VariantName,
                            Price = v.Price,
                            Size = v.Size,
                            Color = v.Color,
                            Stock = v.Stock,
                            Sku = v.Sku,
                            Status = v.Status,
                            ImageUrl = v.ImageUrl,
                        })
                        .ToList();
                }

                return View("~/Views/Shop/Products/Edit.cshtml", viewModel);
            }

            var dto = new UpdateProductDto
            {
                ProductId = id,
                ShopId = shop.Id,
                CategoryId = viewModel.CategoryId,
                Name = viewModel.Name,
                Description = viewModel.Description ?? string.Empty,
                BasePrice = viewModel.BasePrice,
                ImageUrl = viewModel.ImageUrl ?? string.Empty,
            };

            var result = await _productService.UpdateProductAsync(dto);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;

                // Reload data
                var categories = await _productService.GetAllCategoriesAsync();
                viewModel.Categories = categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name,
                        Selected = c.Id == viewModel.CategoryId,
                    })
                    .ToList();

                var detailResult = await _productService.GetProductDetailAsync(id, shop.Id);
                if (detailResult.IsSuccess)
                {
                    viewModel.Variants = detailResult
                        .Data!.Variants.Select(v => new ProductVariantViewModel
                        {
                            Id = v.Id,
                            VariantName = v.VariantName,
                            Price = v.Price,
                            Size = v.Size,
                            Color = v.Color,
                            Stock = v.Stock,
                            Sku = v.Sku,
                            Status = v.Status,
                            ImageUrl = v.ImageUrl,
                        })
                        .ToList();
                }

                return View("~/Views/Shop/Products/Edit.cshtml", viewModel);
            }

            TempData["SuccessMessage"] = "Cập nhật thông tin sản phẩm thành công!";
            return RedirectToAction("Edit", new { id });
        }

        /// <summary>
        /// GET /Products/{id}/Variants/Add - Form thêm biến thể
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AddVariant(Guid id)
        {
            var shop = await GetCurrentUserShopAsync();
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _productService.GetProductDetailAsync(id, shop.Id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Index");
            }

            var product = result.Data!;
            if (product.Status != "draft")
            {
                TempData["ErrorMessage"] =
                    "Chỉ có thể thêm biến thể khi sản phẩm ở trạng thái bản nháp.";
                return RedirectToAction("Edit", new { id });
            }

            var viewModel = new AddVariantViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
            };

            return View("~/Views/Shop/Products/AddVariant.cshtml", viewModel);
        }

        /// <summary>
        /// POST /Products/{id}/Variants/Add - Xử lý thêm biến thể
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVariant(Guid id, AddVariantViewModel viewModel)
        {
            var shop = await GetCurrentUserShopAsync();
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                viewModel.ProductId = id;
                return View("~/Views/Shop/Products/AddVariant.cshtml", viewModel);
            }

            var dto = new CreateProductVariantDto
            {
                ProductId = id,
                VariantName = viewModel.VariantName,
                Price = viewModel.Price,
                Size = viewModel.Size,
                Color = viewModel.Color,
                Stock = viewModel.Stock,
                Sku = viewModel.Sku ?? string.Empty,
                ImageUrl = viewModel.ImageUrl,
            };

            var result = await _productVariantService.AddVariantAsync(dto, shop.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.ErrorMessage ?? "Không thể thêm biến thể."
                );
                viewModel.ProductId = id;
                return View("~/Views/Shop/Products/AddVariant.cshtml", viewModel);
            }

            TempData["SuccessMessage"] = "Thêm biến thể thành công!";
            return RedirectToAction("Edit", new { id });
        }

        /// <summary>
        /// POST /Products/Variants/{variantId}/Delete - Xóa biến thể
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVariant(Guid variantId, Guid productId)
        {
            var shop = await GetCurrentUserShopAsync();
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _productVariantService.DeleteVariantAsync(variantId, shop.Id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }
            else
            {
                TempData["SuccessMessage"] = "Xóa biến thể thành công!";
            }

            return RedirectToAction("Edit", new { id = productId });
        }

        /// <summary>
        /// POST /Products/{id}/Submit - Submit sản phẩm để admin duyệt
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Guid id)
        {
            var shop = await GetCurrentUserShopAsync();
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _productService.SubmitProductAsync(id, shop.Id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Đã gửi sản phẩm để duyệt thành công! Vui lòng chờ admin phê duyệt.";
            }

            return RedirectToAction("Edit", new { id });
        }

        /// <summary>
        /// POST /Products/{id}/Unpublish - Gỡ sản phẩm về trạng thái draft
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpublish(Guid id)
        {
            var shop = await GetCurrentUserShopAsync();
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _productService.UnpublishProductAsync(id, shop.Id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Đã gỡ sản phẩm thành công! Bạn có thể chỉnh sửa và gửi duyệt lại.";
            }

            return RedirectToAction("Edit", new { id });
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
