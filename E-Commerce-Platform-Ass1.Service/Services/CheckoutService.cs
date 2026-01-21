using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.Services.IServices;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemtRepository _orderItemtRepository;

        public CheckoutService(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IOrderRepository orderRepository,
            IOrderItemtRepository orderItemtRepository
        )
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _orderRepository = orderRepository;
            _orderItemtRepository = orderItemtRepository;
        }

        public async Task<Order> CheckoutSuccessAsync(Guid userId, string shippingAddress)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                throw new Exception("Cart is empty");
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ShippingAddress = shippingAddress,
                Status = "PAID",
                CreatedAt = DateTime.Now,
                TotalAmount = cart.CartItems.Sum(ci => ci.ProductVariant.Price * ci.Quantity),
            };

            await _orderRepository.AddAsync(order);

            var orderItems = cart
                .CartItems.Select(ci => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductVariantId = ci.ProductVariantId,
                    ProductName = ci.ProductVariant.VariantName,
                    Price = ci.ProductVariant.Price,
                    Quantity = ci.Quantity,
                })
                .ToList();

            await _orderItemtRepository.AddRangeAsync(orderItems);

            await _cartItemRepository.DeleteByCartIdAsync(cart.Id);

            await _cartRepository.DeleteAsync(cart);

            return order;
        }
    }
}
