using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.Services.IServices;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task AddToCart(Guid userId, Guid productVariantId, int quantity)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart(Guid.NewGuid(), userId);
                await _cartRepository.CreateAsync(cart);
            }

            var cartItem = await _cartItemRepository.GetByCartAndVariantAsync(cart.Id, productVariantId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                await _cartItemRepository.UpdateAsync(cartItem);
            }
            else
            {
                cartItem = new CartItem(Guid.NewGuid(), cart.Id, productVariantId, quantity);
                await _cartItemRepository.AddAsync(cartItem);
            }
        }
    }
}
