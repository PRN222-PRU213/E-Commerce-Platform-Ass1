using System.Diagnostics;
using System.Security.Claims;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IPersonalizationService _personalizationService;

        public HomeController(
            ILogger<HomeController> logger, 
            IProductService productService,
            IPersonalizationService personalizationService)
        {
            _logger = logger;
            _productService = productService;
            _personalizationService = personalizationService;
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            var viewModel = new HomeIndexViewModel
            {
                Products = products
                    .Select(p => new HomeProductItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        BasePrice = p.BasePrice,
                        ImageUrl = p.ImageUrl,
                        AvgRating = p.AvgRating,
                        ShopName = p.ShopName,
                        CategoryName = p.CategoryName,
                    })
                    .ToList(),
            };

            // Get personalized products if user is logged in
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                try
                {
                    var personalizedProducts = await _personalizationService.GetPersonalizedProductsAsync(userId.Value, 8);
                    viewModel.PersonalizedProducts = personalizedProducts
                        .Select(p => new PersonalizedProductViewModel
                        {
                            ProductId = p.ProductId,
                            Name = p.Name,
                            ImageUrl = p.ImageUrl,
                            Price = p.Price,
                            CategoryName = p.CategoryName,
                            ShopName = p.ShopName,
                            RecommendReason = p.RecommendReason
                        })
                        .ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading personalized products for user {UserId}", userId);
                }
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}
