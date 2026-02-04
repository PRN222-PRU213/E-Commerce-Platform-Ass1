using E_Commerce_Platform_Ass1.Service.DTOs;

namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    public interface IUserBehaviorService
    {
        /// <summary>
        /// Track khi user xem sản phẩm
        /// </summary>
        Task TrackViewAsync(Guid userId, Guid productId);
        
        /// <summary>
        /// Track khi user tìm kiếm
        /// </summary>
        Task TrackSearchAsync(Guid userId, string query, Guid? categoryId = null);
        
        /// <summary>
        /// Track khi user thêm vào giỏ hàng
        /// </summary>
        Task TrackAddToCartAsync(Guid userId, Guid productId, int quantity = 1);
        
        /// <summary>
        /// Track khi user hoàn thành đơn hàng
        /// </summary>
        Task TrackPurchaseAsync(Guid userId, Guid orderId);
        
        /// <summary>
        /// Lấy lịch sử hành vi của user
        /// </summary>
        Task<IEnumerable<UserBehaviorDto>> GetUserHistoryAsync(Guid userId, int limit = 50);
    }
}
