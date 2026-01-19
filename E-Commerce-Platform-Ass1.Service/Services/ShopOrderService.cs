using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Models;
using E_Commerce_Platform_Ass1.Service.Services.IServices;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    /// <summary>
    /// Service xử lý đơn hàng cho Shop Owner
    /// </summary>
    public class ShopOrderService : IShopOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IUserRepository _userRepository;

        public ShopOrderService(
            IOrderRepository orderRepository,
            IShipmentRepository shipmentRepository,
            IUserRepository userRepository
        )
        {
            _orderRepository = orderRepository;
            _shipmentRepository = shipmentRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResult<List<OrderDto>>> GetOrdersByShopIdAsync(Guid shopId)
        {
            var orders = await _orderRepository.GetByShopIdAsync(shopId);

            var orderDtos = orders
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    CustomerName = o.User?.Name,
                    CustomerEmail = o.User?.Email,
                    TotalAmount = o.TotalAmount,
                    ShippingAddress = o.ShippingAddress,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    ItemCount = o.OrderItems.Count,
                    Carrier = o.Shipments?.FirstOrDefault()?.Carrier,
                    TrackingCode = o.Shipments?.FirstOrDefault()?.TrackingCode,
                    ShipmentStatus = o.Shipments?.FirstOrDefault()?.Status,
                })
                .ToList();

            return ServiceResult<List<OrderDto>>.Success(orderDtos);
        }

        public async Task<ServiceResult<OrderDetailDto>> GetOrderDetailAsync(
            Guid orderId,
            Guid shopId
        )
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null)
            {
                return ServiceResult<OrderDetailDto>.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra đơn hàng có thuộc shop này không
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult<OrderDetailDto>.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            var shipment = order.Shipments?.FirstOrDefault();
            var payment = order.Payments?.FirstOrDefault();

            var dto = new OrderDetailDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CustomerName = order.User?.Name,
                CustomerEmail = order.User?.Email,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                ItemCount = order.OrderItems.Count,
                Carrier = shipment?.Carrier,
                TrackingCode = shipment?.TrackingCode,
                ShipmentStatus = shipment?.Status,
                Items = order
                    .OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        ProductVariantId = oi.ProductVariantId,
                        ProductName = oi.ProductName,
                        VariantName = oi.ProductVariant?.VariantName,
                        Size = oi.ProductVariant?.Size,
                        Color = oi.ProductVariant?.Color,
                        ImageUrl = oi.ProductVariant?.Product?.ImageUrl,
                        Price = oi.Price,
                        Quantity = oi.Quantity,
                    })
                    .ToList(),
                Payment =
                    payment != null
                        ? new PaymentDto
                        {
                            Id = payment.Id,
                            Method = payment.Method,
                            Amount = payment.Amount,
                            Status = payment.Status,
                            TransactionCode = payment.TransactionCode,
                            PaidAt = payment.PaidAt,
                        }
                        : null,
                Shipment =
                    shipment != null
                        ? new ShipmentDto
                        {
                            Id = shipment.Id,
                            Carrier = shipment.Carrier,
                            TrackingCode = shipment.TrackingCode,
                            Status = shipment.Status,
                            UpdatedAt = shipment.UpdatedAt,
                        }
                        : null,
            };

            return ServiceResult<OrderDetailDto>.Success(dto);
        }

        public async Task<ServiceResult> ConfirmOrderAsync(Guid orderId, Guid shopId)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra quyền
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            if (order.Status != "Pending")
            {
                return ServiceResult.Failure("Chỉ có thể xác nhận đơn hàng đang chờ xử lý.");
            }

            order.Status = "Confirmed";
            await _orderRepository.UpdateAsync(order);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> UpdateShipmentAsync(
            Guid orderId,
            Guid shopId,
            UpdateShipmentDto dto
        )
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra quyền
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            if (order.Status != "Confirmed" && order.Status != "Processing")
            {
                return ServiceResult.Failure("Đơn hàng chưa được xác nhận hoặc đã hoàn thành.");
            }

            // Cập nhật trạng thái order
            order.Status = "Processing";
            await _orderRepository.UpdateAsync(order);

            // Tạo hoặc cập nhật shipment
            var shipment = order.Shipments?.FirstOrDefault();
            if (shipment == null)
            {
                shipment = new Shipment
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    Carrier = dto.Carrier,
                    TrackingCode = dto.TrackingCode,
                    Status = dto.Status,
                    UpdatedAt = DateTime.Now,
                };
                await _shipmentRepository.AddAsync(shipment);
            }
            else
            {
                shipment.Carrier = dto.Carrier;
                shipment.TrackingCode = dto.TrackingCode;
                shipment.Status = dto.Status;
                shipment.UpdatedAt = DateTime.Now;
                await _shipmentRepository.UpdateAsync(shipment);
            }

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> MarkAsDeliveredAsync(Guid orderId, Guid shopId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra quyền
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            // Cập nhật order
            order.Status = "Completed";
            await _orderRepository.UpdateAsync(order);

            // Cập nhật shipment
            var shipment = order.Shipments?.FirstOrDefault();
            if (shipment != null)
            {
                shipment.Status = "Delivered";
                shipment.UpdatedAt = DateTime.Now;
                await _shipmentRepository.UpdateAsync(shipment);
            }

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RejectOrderAsync(Guid orderId, Guid shopId, string? reason)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra quyền
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            if (order.Status != "Pending" && order.Status != "Confirmed")
            {
                return ServiceResult.Failure("Không thể từ chối đơn hàng đã được xử lý.");
            }

            order.Status = "Cancelled";
            await _orderRepository.UpdateAsync(order);

            return ServiceResult.Success();
        }

        public async Task<ShopOrderStatistics> GetOrderStatisticsAsync(Guid shopId)
        {
            var orders = await _orderRepository.GetByShopIdAsync(shopId);
            var orderList = orders.ToList();

            return new ShopOrderStatistics
            {
                TotalOrders = orderList.Count,
                PendingOrders = orderList.Count(o => o.Status == "Pending"),
                ConfirmedOrders = orderList.Count(o => o.Status == "Confirmed"),
                ShippingOrders = orderList.Count(o => o.Status == "Processing"),
                DeliveredOrders = orderList.Count(o => o.Status == "Completed"),
                CancelledOrders = orderList.Count(o => o.Status == "Cancelled"),
                TotalRevenue = orderList
                    .Where(o => o.Status == "Completed")
                    .Sum(o => o.TotalAmount),
            };
        }
    }
}
