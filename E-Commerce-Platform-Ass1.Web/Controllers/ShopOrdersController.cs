using System.Security.Claims;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    /// <summary>
    /// Controller quản lý đơn hàng cho Shop Owner
    /// </summary>
    [Authorize]
    public class ShopOrdersController : Controller
    {
        private readonly IShopOrderService _shopOrderService;
        private readonly IShopRepository _shopRepository;

        public ShopOrdersController(
            IShopOrderService shopOrderService,
            IShopRepository shopRepository)
        {
            _shopOrderService = shopOrderService;
            _shopRepository = shopRepository;
        }

        /// <summary>
        /// Lấy ShopId của user hiện tại
        /// </summary>
        private async Task<Guid?> GetCurrentShopIdAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return null;
            }

            var shop = await _shopRepository.GetByUserIdAsync(userId);
            return shop?.Id;
        }

        /// <summary>
        /// GET /ShopOrders - Danh sách đơn hàng của shop
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string? status = null)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("RegisterShop", "Shop");
            }

            var result = await _shopOrderService.GetOrdersByShopIdAsync(shopId.Value);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return View(new List<OrderDto>());
            }

            var orders = result.Data ?? new List<OrderDto>();

            // Lọc theo status nếu có
            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.Status == status).ToList();
            }

            // Lấy thống kê
            var statistics = await _shopOrderService.GetOrderStatisticsAsync(shopId.Value);
            ViewBag.Statistics = statistics;
            ViewBag.CurrentStatus = status;

            return View(orders);
        }

        /// <summary>
        /// GET /ShopOrders/Detail/{id} - Chi tiết đơn hàng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("RegisterShop", "Shop");
            }

            var result = await _shopOrderService.GetOrderDetailAsync(id, shopId.Value);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Index");
            }

            return View(result.Data);
        }

        /// <summary>
        /// POST /ShopOrders/Confirm/{id} - Xác nhận đơn hàng
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index");
            }

            var result = await _shopOrderService.ConfirmOrderAsync(id, shopId.Value);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã xác nhận đơn hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// POST /ShopOrders/UpdateShipment/{id} - Cập nhật trạng thái vận chuyển
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateShipment(Guid id, UpdateShipmentDto dto)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index");
            }

            var result = await _shopOrderService.UpdateShipmentAsync(id, shopId.Value, dto);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã cập nhật thông tin vận chuyển thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// POST /ShopOrders/MarkDelivered/{id} - Đánh dấu đã giao hàng
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDelivered(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index");
            }

            var result = await _shopOrderService.MarkAsDeliveredAsync(id, shopId.Value);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã đánh dấu giao hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// POST /ShopOrders/Reject/{id} - Từ chối đơn hàng
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id, string? reason)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToAction("Index");
            }

            var result = await _shopOrderService.RejectOrderAsync(id, shopId.Value, reason);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã từ chối đơn hàng!";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToAction("Index");
        }
    }
}
