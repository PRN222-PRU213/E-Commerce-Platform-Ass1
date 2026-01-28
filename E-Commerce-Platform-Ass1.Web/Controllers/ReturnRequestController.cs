using System.Security.Claims;
using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    /// <summary>
    /// Controller xử lý yêu cầu đổi trả/hoàn tiền cho Customer
    /// </summary>
    [Authorize]
    public class ReturnRequestController : Controller
    {
        private readonly IReturnRequestService _returnRequestService;
        private readonly IOrderService _orderService;
        private readonly ICloudinaryService _cloudinaryService;

        public ReturnRequestController(
            IReturnRequestService returnRequestService,
            IOrderService orderService,
            ICloudinaryService cloudinaryService)
        {
            _returnRequestService = returnRequestService;
            _orderService = orderService;
            _cloudinaryService = cloudinaryService;
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// GET: /ReturnRequest - Danh sách yêu cầu của tôi
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var requests = await _returnRequestService.GetMyRequestsAsync(userId.Value);
            return View(requests);
        }

        /// <summary>
        /// GET: /ReturnRequest/Create/{orderId} - Form tạo yêu cầu
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(Guid orderId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            // Check if can create request
            var canCreate = await _returnRequestService.CanCreateRequestAsync(orderId, userId.Value);
            if (!canCreate)
            {
                TempData["Error"] = "Không thể tạo yêu cầu hoàn trả cho đơn hàng này.";
                return RedirectToAction("History", "Order");
            }

            var order = await _orderService.GetOrderItemAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new CreateReturnRequestViewModel
            {
                OrderId = orderId,
                OrderDate = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                RequestedAmount = order.TotalAmount, // Default to full refund
                Items = order.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.ProductName,
                    ImageUrl = i.ImageUrl ?? string.Empty,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    Size = i.Size,
                    Color = i.Color
                }).ToList()
            };

            return View(viewModel);
        }

        /// <summary>
        /// POST: /ReturnRequest/Create - Xử lý tạo yêu cầu
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReturnRequestViewModel model, List<IFormFile>? evidenceImages)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            if (!ModelState.IsValid)
            {
                // Reload order items for view
                var order = await _orderService.GetOrderItemAsync(model.OrderId);
                if (order != null)
                {
                    model.Items = order.Items.Select(i => new OrderItemViewModel
                    {
                        ProductName = i.ProductName,
                        ImageUrl = i.ImageUrl ?? string.Empty,
                        Price = i.Price,
                        Quantity = i.Quantity,
                        Size = i.Size,
                        Color = i.Color
                    }).ToList();
                    model.TotalAmount = order.TotalAmount;
                    model.OrderDate = order.CreatedAt;
                }
                return View(model);
            }

            // Upload evidence images
            var imageUrls = new List<string>();
            if (evidenceImages != null && evidenceImages.Any())
            {
                foreach (var image in evidenceImages.Take(5)) // Max 5 images
                {
                    if (image.Length > 0)
                    {
                        var imageUrl = await _cloudinaryService.UploadImageAsync(image, "return-requests");
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            imageUrls.Add(imageUrl);
                        }
                    }
                }
            }

            var dto = new CreateReturnRequestDto
            {
                OrderId = model.OrderId,
                UserId = userId.Value,
                RequestType = model.RequestType,
                Reason = model.Reason,
                ReasonDetail = model.ReasonDetail,
                EvidenceImageUrls = imageUrls,
                RequestedAmount = model.RequestedAmount
            };

            var result = await _returnRequestService.CreateRequestAsync(dto);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Yêu cầu hoàn trả đã được gửi thành công! Vui lòng chờ xét duyệt.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = result.ErrorMessage;
            
            // Reload for view
            var orderReload = await _orderService.GetOrderItemAsync(model.OrderId);
            if (orderReload != null)
            {
                model.Items = orderReload.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.ProductName,
                    ImageUrl = i.ImageUrl ?? string.Empty,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    Size = i.Size,
                    Color = i.Color
                }).ToList();
                model.TotalAmount = orderReload.TotalAmount;
                model.OrderDate = orderReload.CreatedAt;
            }
            
            return View(model);
        }

        /// <summary>
        /// GET: /ReturnRequest/Detail/{id} - Chi tiết yêu cầu
        /// </summary>
        public async Task<IActionResult> Detail(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var request = await _returnRequestService.GetRequestDetailAsync(id, userId.Value);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }
    }
}
