using System.Security.Claims;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> Reorder(Guid orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var order = await _orderService.GetOrderItemAsync(orderId);
            if (order != null && order.Status == "Cancelled")
            {
                foreach (var item in order.Items)
                {
                    await _cartService.AddToCart(userId, item.ProductVariantId, item.Quantity);
                }
                TempData["Success"] = "Đã thêm các sản phẩm từ đơn hàng cũ vào giỏ hàng của bạn!";
            }
            else
            {
                TempData["Error"] = "Không thể thực hiện mua lại cho đơn hàng này.";
                return RedirectToAction("History");
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public async Task<IActionResult> History(int page = 1)
        {
            const int pageSize = 5;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction(
                    "Login",
                    "Authentication",
                    new { returnUrl = Url.Action("Index", "Home") }
                );
            }

            var orders = await _orderService.GetOrderHistoryAsync(userId);

            var model = orders.Select(o => new OrderHistoryViewModel
            {
                OrderId = o.Id,
                OrderDate = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                ShippingAddress = o.ShippingAddress,
                IsRefunded = false
            })
            .OrderByDescending(o => o.OrderDate)
            .ToList();

            var pagedResult = new PagedResult<OrderHistoryViewModel>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = model.Count,
                Items = model
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
            };

            return View("OrderHistory", pagedResult);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction(
                    "Login",
                    "Authentication",
                    new { returnUrl = Url.Action("Index", "Home") }
                );
            }

            var order = await _orderService.GetOrderItemAsync(orderId);

            if (order == null)
                return NotFound();

            var orderDetail = new OrderDetailViewModel
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(o => new OrderItemViewModel
                {
                    ProductName = o.ProductName,
                    ImageUrl = o.ImageUrl ?? string.Empty,
                    Price = o.Price,
                    Quantity = o.Quantity,
                    Size = o.Size,
                    Color = o.Color
                }).ToList()
            };

            return View("OrderDetail", orderDetail);
        }

        public IActionResult OrderHistory()
        {
            return View();
        }
    }
}
