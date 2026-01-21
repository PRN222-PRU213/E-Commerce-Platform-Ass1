using E_Commerce_Platform_Ass1.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    [Route("[controller]")]
    public class CartItemController : Controller
    {
        private readonly ICartService _cartService;

        public CartItemController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveItemFromCartAsync(Guid id)
        {
            try
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

                var isDeleted = await _cartService.RemoveItemAsync(userId, id);

                if (!isDeleted) return NotFound(new { message = "Sản phẩm không tồn tại." });

                var newCartTotal = await _cartService.GetCartTotalAsync(userId);
                var newItemCount = await _cartService.GetTotalItemCountAsync(userId);

                return Ok(new
                {
                    success = true,
                    message = "Đã xóa sản phẩm khỏi giỏ hàng.",
                    newCartTotal = newCartTotal,
                    newItemCount = newItemCount
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateQuantityItemAsync(Guid cartItemId, int quantity)
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

            var isUpdated = await _cartService.UpdateQuantityAsync(cartItemId, quantity);
            if (!isUpdated) return NotFound(new { message = "Sản phẩm không tồn tại." });
            
            var updatedItem = await _cartService.GetCartItemAsync(cartItemId);
            var cartTotal = await _cartService.GetCartTotalAsync(userId);

            return Ok(new
            {
                success = true,
                quantity = updatedItem.Quantity,
                lineTotal = updatedItem.Quantity * updatedItem.ProductVariant.Price,
                cartTotal = cartTotal
            });
        }
    }
}
