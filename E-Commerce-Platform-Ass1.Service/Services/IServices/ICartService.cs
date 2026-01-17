using E_Commerce_Platform_Ass1.Service.DTO.Response;

namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    public interface ICartService
    {
        Task AddToCart(Guid userId, Guid productVariantId, int quantity);

        Task<CartViewModel?> GetCartUserAsync(Guid userId);

        Task<int> GetTotalItemCountAsync(Guid userId);
    }
}
