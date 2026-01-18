using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Models;

namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    /// <summary>
    /// Service xử lý đơn hàng cho Shop Owner
    /// </summary>
    public interface IShopOrderService
    {
        /// <summary>
        /// Lấy danh sách đơn hàng của shop
        /// </summary>
        Task<ServiceResult<List<OrderDto>>> GetOrdersByShopIdAsync(Guid shopId);

        /// <summary>
        /// Lấy chi tiết đơn hàng
        /// </summary>
        Task<ServiceResult<OrderDetailDto>> GetOrderDetailAsync(Guid orderId, Guid shopId);

        /// <summary>
        /// Xác nhận đơn hàng
        /// </summary>
        Task<ServiceResult> ConfirmOrderAsync(Guid orderId, Guid shopId);

        /// <summary>
        /// Cập nhật trạng thái vận chuyển
        /// </summary>
        Task<ServiceResult> UpdateShipmentAsync(Guid orderId, Guid shopId, UpdateShipmentDto dto);

        /// <summary>
        /// Đánh dấu đã giao hàng
        /// </summary>
        Task<ServiceResult> MarkAsDeliveredAsync(Guid orderId, Guid shopId);

        /// <summary>
        /// Từ chối đơn hàng
        /// </summary>
        Task<ServiceResult> RejectOrderAsync(Guid orderId, Guid shopId, string? reason);

        /// <summary>
        /// Lấy thống kê đơn hàng của shop
        /// </summary>
        Task<ShopOrderStatistics> GetOrderStatisticsAsync(Guid shopId);
    }

    /// <summary>
    /// Thống kê đơn hàng shop
    /// </summary>
    public class ShopOrderStatistics
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
