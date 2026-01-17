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
