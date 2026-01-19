using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    public class CheckoutController : Controller
    {
        [HttpGet]
        public IActionResult PaymentCallBack(
        int resultCode,
        string message,
        string orderId,
        long amount)
        {
            if (resultCode == 0)
            {
                ViewBag.IsSuccess = true;
                ViewBag.Message = "Thanh toán thành công!";
            }
            else
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = message ?? "Thanh toán không thành công";
            }

            return View();
        }
    }
}
