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
        private readonly IPaymentRepository _paymentRepository;

        public CheckoutService(ICartRepository cartRepository, ICartItemRepository cartItemRepository,
            IOrderRepository orderRepository, IOrderItemtRepository orderItemtRepository,
            IPaymentRepository paymentRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _orderRepository = orderRepository;
            _orderItemtRepository = orderItemtRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<Order> CheckoutSuccessAsync(Guid userId, string shippingAddress, List<Guid> selectedCartItemIds)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                throw new Exception("Cart is empty");
            }

            var selectItems = cart.CartItems
                .Where(ci => selectedCartItemIds.Contains(ci.Id))
                .ToList();

            if (!selectItems.Any())
            {
                throw new Exception("No selected items");
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ShippingAddress = shippingAddress,
                Status = "PAID",
                CreatedAt = DateTime.Now,
                TotalAmount = selectItems.Sum(ci =>
                    ci.ProductVariant.Price * ci.Quantity)
            };

            await _orderRepository.AddAsync(order);

            var orderItems = selectItems.Select(ci => new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductVariantId = ci.ProductVariantId,
                ProductName = ci.ProductVariant.VariantName,
                Price = ci.ProductVariant.Price,
                Quantity = ci.Quantity
            }).ToList();

            await _orderItemtRepository.AddRangeAsync(orderItems);

            var payment = new Payment(order.Id, order.TotalAmount);

            await _paymentRepository.AddAsync(payment);

            await _cartItemRepository.DeleteByIdsAsync(
                selectItems.Select(ci => ci.Id).ToList());

            var remainingItems = cart.CartItems
                .Where(ci => !selectedCartItemIds.Contains(ci.Id))
                .ToList();

            if (!remainingItems.Any())
            {
                await _cartRepository.DeleteAsync(cart);
            }

            return order;
        }
    }
}
