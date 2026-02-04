using E_Commerce_Platform_Ass1.Service.DTOs;

namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    public interface IPersonalizationService
    {
        /// <summary>
        /// Lấy sản phẩm được cá nhân hóa cho user (trang chủ)
        /// </summary>
        Task<IEnumerable<PersonalizedProductDto>> GetPersonalizedProductsAsync(Guid userId, int limit = 12);
        
        /// <summary>
        /// Lấy sản phẩm liên quan dựa trên sản phẩm đang xem
        /// </summary>
        Task<IEnumerable<PersonalizedProductDto>> GetRelatedProductsAsync(Guid userId, Guid currentProductId, int limit = 6);
        
        /// <summary>
        /// Sản phẩm "Thường được mua cùng" 
        /// </summary>
        Task<IEnumerable<PersonalizedProductDto>> GetFrequentlyBoughtTogetherAsync(Guid productId, int limit = 4);
        
        /// <summary>
        /// Phân tích và cập nhật user preferences bằng AI
        /// </summary>
        Task AnalyzeUserPreferencesAsync(Guid userId);
        
        /// <summary>
        /// Lấy user preferences hiện tại
        /// </summary>
        Task<UserPreferenceDto?> GetUserPreferencesAsync(Guid userId);
    }
}
