using System.Text.Json;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Services.IServices;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    public class UserBehaviorService : IUserBehaviorService
    {
        private readonly IUserBehaviorRepository _behaviorRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public UserBehaviorService(
            IUserBehaviorRepository behaviorRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository)
        {
            _behaviorRepository = behaviorRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public async Task TrackViewAsync(Guid userId, Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return;

            var behavior = new UserBehavior
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductId = productId,
                CategoryId = product.CategoryId,
                ActionType = "View",
                CreatedAt = DateTime.UtcNow
            };

            await _behaviorRepository.AddAsync(behavior);
        }

        public async Task TrackSearchAsync(Guid userId, string query, Guid? categoryId = null)
        {
            if (string.IsNullOrWhiteSpace(query)) return;

            var behavior = new UserBehavior
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = categoryId,
                ActionType = "Search",
                SearchQuery = query.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _behaviorRepository.AddAsync(behavior);
        }

        public async Task TrackAddToCartAsync(Guid userId, Guid productId, int quantity = 1)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return;

            var behavior = new UserBehavior
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductId = productId,
                CategoryId = product.CategoryId,
                ActionType = "AddToCart",
                Metadata = JsonSerializer.Serialize(new { quantity }),
                CreatedAt = DateTime.UtcNow
            };

            await _behaviorRepository.AddAsync(behavior);
        }

        public async Task TrackPurchaseAsync(Guid userId, Guid orderId)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            if (order == null) return;

            foreach (var item in order.OrderItems)
            {
                var behavior = new UserBehavior
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProductId = item.ProductVariant?.ProductId,
                    CategoryId = item.ProductVariant?.Product?.CategoryId,
                    ActionType = "Purchase",
                    Metadata = JsonSerializer.Serialize(new 
                    { 
                        orderId, 
                        quantity = item.Quantity, 
                        price = item.Price 
                    }),
                    CreatedAt = DateTime.UtcNow
                };

                await _behaviorRepository.AddAsync(behavior);
            }
        }

        public async Task<IEnumerable<UserBehaviorDto>> GetUserHistoryAsync(Guid userId, int limit = 50)
        {
            var behaviors = await _behaviorRepository.GetByUserIdAsync(userId, limit);
            
            return behaviors.Select(b => new UserBehaviorDto
            {
                ProductId = b.ProductId ?? Guid.Empty,
                ProductName = b.Product?.Name ?? string.Empty,
                CategoryId = b.CategoryId,
                CategoryName = b.Category?.Name ?? b.Product?.Category?.Name,
                ActionType = b.ActionType,
                SearchQuery = b.SearchQuery,
                ProductPrice = b.Product?.BasePrice,
                CreatedAt = b.CreatedAt
            });
        }
    }
}
