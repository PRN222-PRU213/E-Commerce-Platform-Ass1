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
        private readonly IUserService _userService;

        public CheckoutController(ICheckoutService checkoutService, IUserService userService)
        {
            _checkoutService = checkoutService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallBack(
            int resultCode,
            string message)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = "Không xác định được người dùng.";
                return View();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            ViewBag.UserName = user.Name;

            if (resultCode == 0)
            {
                // ✅ LẤY ĐỊA CHỈ TỪ SESSION
                var shippingAddress = HttpContext.Session.GetString("ShippingAddress");

                if (string.IsNullOrEmpty(shippingAddress))
                {
                    ViewBag.IsSuccess = false;
                    ViewBag.Message = "Không tìm thấy địa chỉ giao hàng.";
                    return View();
                }

                var newOrder = await _checkoutService.CheckoutSuccessAsync(userId, shippingAddress);

                // Xóa session sau khi dùng
                HttpContext.Session.Remove("ShippingAddress");

                ViewBag.IsSuccess = true;
                ViewBag.Message = $"Thanh toán thành công! Mã đơn hàng: {newOrder.Id}\n" + 
                    $"Tổng tiền: {newOrder.TotalAmount}";
            }
            else
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = resultCode switch
                {
                    1006 => "Bạn đã hủy thanh toán.",
                    1005 => "Tài khoản không đủ số dư.",
                    1003 => "Giao dịch đã hết hạn.",
                    _ => "Thanh toán không thành công. Vui lòng thử lại."
                };
            }

            return View("PaymentCallBack");
        }
    }
}
