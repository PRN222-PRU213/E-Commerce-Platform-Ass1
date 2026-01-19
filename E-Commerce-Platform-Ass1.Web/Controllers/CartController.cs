using E_Commerce_Platform_Ass1.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Authentication", new { returnUrl = Url.Action("Index", "Cart") });
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var serviceCart = await _cartService.GetCartUserAsync(userId);

            var cartVM = new Models.CartViewModel
            {
                Items = serviceCart?.Items.Select(i => new Models.CartItemViewModel
                {
                    CartItemId = i.CartItemId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ImageUrl = i.ImageUrl,
                    Price = i.Price,
                    Quantity = i.Stock,
                    Size = i.Size,
                    Color = i.Color,
                    TotalLinePrice = i.TotalLinePrice
                }).ToList() ?? new List<Models.CartItemViewModel>(),
                TotalPrice = serviceCart?.TotalPrice ?? 0,
                Shipping = 0
            };

            return View(cartVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid productId, Guid productVariantId, int quantity)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Authentication", new { returnUrl = Url.Action("Detail", "Product", new { id = productId }) });
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            await _cartService.AddToCart(userId, productVariantId, quantity);

            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";

            return RedirectToAction("Detail", "Product", new { id = productId });
        }
    }
}
