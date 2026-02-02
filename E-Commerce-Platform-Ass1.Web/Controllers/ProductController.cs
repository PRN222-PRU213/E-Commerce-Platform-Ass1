using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUserBehaviorService _behaviorService;

        public ProductController(IProductService productService, IUserBehaviorService behaviorService)
        {
            _productService = productService;
            _behaviorService = behaviorService;
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var productDto = await _productService.GetProductDetailDtoAsync(id);
            if (productDto == null)
            {
                return NotFound();
            }

            // Track user viewing this product for AI personalization
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                try
                {
                    await _behaviorService.TrackViewAsync(userId.Value, id);
                }
                catch (Exception)
                {
                    throw new Exception("Không thể theo dõi hành vi của người dùng ở product detail.");
                }
            }

            var viewModel = new ProductDetailViewModel
            {
                Id = productDto.Id,
                ShopId = productDto.ShopId,
                CategoryId = productDto.CategoryId,
                Name = productDto.Name,
                Description = productDto.Description,
                BasePrice = productDto.BasePrice,
                Status = productDto.Status,
                AvgRating = productDto.AvgRating,
                ImageUrl = productDto.ImageUrl,
                CreatedAt = productDto.CreatedAt,
                CategoryName = productDto.CategoryName,
                ShopName = productDto.ShopName,
                Variants = productDto
                    .Variants.Select(v => new ProductVariantItemViewModel
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

            return View(viewModel);
        }

        public async Task<IActionResult> QuickView(Guid id)
        {
            var productDto = await _productService.GetProductDetailDtoAsync(id);
            if (productDto == null)
            {
                return NotFound();
            }

            var viewModel = new ProductDetailViewModel
            {
                Id = productDto.Id,
                ShopId = productDto.ShopId,
                CategoryId = productDto.CategoryId,
                Name = productDto.Name,
                Description = productDto.Description,
                BasePrice = productDto.BasePrice,
                Status = productDto.Status,
                AvgRating = productDto.AvgRating,
                ImageUrl = productDto.ImageUrl,
                CreatedAt = productDto.CreatedAt,
                CategoryName = productDto.CategoryName,
                ShopName = productDto.ShopName,
                Variants = productDto
                    .Variants.Select(v => new ProductVariantItemViewModel
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

            return PartialView("_ProductVariantPopup", viewModel);
        }
    }
}
