namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    public interface ICartService
    {
        Task AddToCart(Guid userId, Guid productVariantId, int quantity);
    }
}
