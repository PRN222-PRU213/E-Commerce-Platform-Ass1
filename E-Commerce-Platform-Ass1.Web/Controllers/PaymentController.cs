using System.Security.Claims;
using E_Commerce_Platform_Ass1.Service.Services;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IMomoService _momoService;
        private readonly ICartService _cartService;

        public PaymentController(IMomoService momoService, ICartService cartService)
        {
            _momoService = momoService;
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> PayWithMomo(string shippingAddress, string selectedCartItemIds)
        {
            if (string.IsNullOrWhiteSpace(selectedCartItemIds))
            {
                TempData["Error"] = "Vui lòng chọn sản phẩm để thanh toán";
                return RedirectToAction("Index", "Cart");
            }

            var cartItemIds = selectedCartItemIds
                .Split(',')
                .Select(Guid.Parse)
                .ToList();

            var selectedItems = await _cartService.GetCartItemsByIdsAsync(cartItemIds);

            var totalAmount = selectedItems.Sum(x => x.Quantity * x.ProductVariant.Price);

            if (string.IsNullOrWhiteSpace(shippingAddress))
            {
                TempData["Error"] = "Vui lòng nhập địa chỉ giao hàng";
                return RedirectToAction("Index", "Cart");
            }

            // ✅ Lưu vào Session
            HttpContext.Session.SetString("ShippingAddress", shippingAddress);
            HttpContext.Session.SetString("SelectedCartItemIds", selectedCartItemIds);

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cart = await _cartService.GetCartUserAsync(userId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToAction("Index", "Cart");
            }

            long amount = (long)totalAmount;

            if (amount <= 0)
            {
                TempData["Error"] = "Số tiền không hợp lệ";
                return RedirectToAction("Index", "Cart");
            }

            var payUrl = await _momoService.CreatePaymentAsync(
                amount,
                "Thanh toán đơn hàng");

            if (string.IsNullOrEmpty(payUrl))
            {
                TempData["Error"] = "Thanh toán MoMo thất bại";
                return RedirectToAction("Index", "Cart");
            }

            return Redirect(payUrl);
        }

        [HttpGet]
        public IActionResult Result()
        {
            return View();
        }
    }
}
