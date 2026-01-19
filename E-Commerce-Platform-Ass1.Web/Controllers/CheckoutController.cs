using System.Security.Claims;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallBack(
            int resultCode,
            string message)
        {
            if (resultCode == 0)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userIdStr, out Guid userId))
                {
                    ViewBag.IsSuccess = false;
                    ViewBag.Message = "Không xác định được người dùng.";
                    return View();
                }

                // ✅ LẤY ĐỊA CHỈ TỪ SESSION
                var shippingAddress = HttpContext.Session.GetString("ShippingAddress");

                if (string.IsNullOrEmpty(shippingAddress))
                {
                    ViewBag.IsSuccess = false;
                    ViewBag.Message = "Không tìm thấy địa chỉ giao hàng.";
                    return View();
                }

                var newOrderId = await _checkoutService.CheckoutSuccessAsync(userId, shippingAddress);

                // Xóa session sau khi dùng
                HttpContext.Session.Remove("ShippingAddress");

                ViewBag.IsSuccess = true;
                ViewBag.Message = $"Thanh toán thành công! Mã đơn hàng: {newOrderId}";
            }
            else
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = message ?? "Thanh toán không thành công";
            }

            return View("PaymentCallBack");
        }
    }
}
