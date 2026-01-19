using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Models;
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
                // In a real app, you might want to show an empty cart or redirect to login.
                // For this demo, let's redirect to login if we need to fetch user-specific data.
                return RedirectToAction(
                    "Login",
                    "Authentication",
                    new { returnUrl = Url.Action("Index", "Cart") }
                );
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var serviceCart = await _cartService.GetCartUserAsync(userId);

            var cartVM = new Models.CartViewModel
            {
                Items =
                    serviceCart
                        ?.Items.Select(i => new Models.CartItemViewModel
                        {
                            ProductId = i.ProductId,
                            ProductName = i.ProductName,
                            ImageUrl = i.ImageUrl,
                            Price = i.Price,
                            Quantity = i.Stock, // Service DTO uses Stock for quantity in cart
                            Size = i.Size,
                            Color = i.Color,
                            TotalLinePrice = i.TotalLinePrice,
                        })
                        .ToList() ?? new List<Models.CartItemViewModel>(),
                TotalPrice = serviceCart?.TotalPrice ?? 0,
                Shipping = 0, // Shipping is currently not calculated/required by the user request
            };

            return View(cartVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(
            Guid productId,
            Guid productVariantId,
            int quantity
        )
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction(
                    "Login",
                    "Authentication",
                    new { returnUrl = Url.Action("Detail", "Product", new { id = productId }) }
                );
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
