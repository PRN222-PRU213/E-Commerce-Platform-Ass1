using E_Commerce_Platform_Ass1.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    public class RefundController : Controller
    {
        private readonly IRefundService _refundService;

        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }

        [HttpPost()]
        public async Task<IActionResult> Refund(
            Guid orderId,
            decimal amount,
            string reason)
        {
            await _refundService.RefundAsync(orderId, amount, reason);
            TempData["Success"] = "Hoàn tiền thành công";

            return RedirectToAction("History", "Order");
        }
    }
}
