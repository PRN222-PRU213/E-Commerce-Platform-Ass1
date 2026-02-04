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
        private readonly IShopWalletService _shopWalletService;
        private readonly IUserBehaviorService _behaviorService;

        public CheckoutController(
            ICheckoutService checkoutService, 
            IUserService userService,
            IShopWalletService shopWalletService,
            IUserBehaviorService behaviorService)
        {
            _checkoutService = checkoutService;
            _userService = userService;
            _shopWalletService = shopWalletService;
            _behaviorService = behaviorService;
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallBack(
            int resultCode)
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
                var shippingAddress = HttpContext.Session.GetString("ShippingAddress");
                var selectedIdsStr = HttpContext.Session.GetString("SelectedCartItemIds");

                var walletUsedStr = HttpContext.Session.GetString("WalletUsed");
                var momoAmountStr = HttpContext.Session.GetString("MomoAmount");

                if (string.IsNullOrEmpty(shippingAddress) ||
                    string.IsNullOrEmpty(selectedIdsStr) ||
                    string.IsNullOrEmpty(walletUsedStr))
                {
                    ViewBag.IsSuccess = false;
                    ViewBag.Message = "Thiếu thông tin thanh toán.";
                    return View();
                }

                var selectedCartItemIds = selectedIdsStr
                    .Split(',')
                    .Select(Guid.Parse)
                    .ToList();

                decimal walletUsed = decimal.Parse(walletUsedStr);
                decimal momoAmount = decimal.Parse(momoAmountStr ?? "0");

                
                var newOrder = await _checkoutService.ConfirmPaymentAsync(
                    userId,
                    shippingAddress,
                    selectedCartItemIds,
                    walletUsed,
                    momoAmount
                );

                
                await _shopWalletService.DistributeOrderPaymentAsync(newOrder.Id);

                
                try
                {
                    await _behaviorService.TrackPurchaseAsync(userId, newOrder.Id);
                }
                catch (Exception)
                {
                    throw new Exception("Không thể theo dõi hành vi của người dùng ở checkout.");
                }

                HttpContext.Session.Clear();

                ViewBag.IsSuccess = true;
                ViewBag.Message =
                    $"Thanh toán thành công!<br/>" +
                    $"Mã đơn hàng: {newOrder.Id}<br/>" +
                    $"Tổng tiền: {newOrder.TotalAmount:N0} VNĐ";
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
