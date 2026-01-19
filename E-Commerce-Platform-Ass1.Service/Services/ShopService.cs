using System;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.Services.IServices;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository _shopRepository;

        public ShopService(IShopRepository shopRepository)
        {
            _shopRepository = shopRepository;
        }

        public async Task<Shop?> GetShopByUserIdAsync(Guid userId)
        {
            return await _shopRepository.GetByUserIdAsync(userId);
        }

        public async Task<Shop?> GetShopByIdAsync(Guid shopId)
        {
            return await _shopRepository.GetByIdAsync(shopId);
        }

        public async Task<bool> UserHasShopAsync(Guid userId)
        {
            return await _shopRepository.ExistsByUserId(userId);
        }

        public async Task<bool> ShopNameExistsAsync(string shopName)
        {
            return await _shopRepository.ExistsByShopName(shopName);
        }

        public async Task<Shop> RegisterShopAsync(Guid userId, string shopName, string description)
        {
            var shop = new Shop
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ShopName = shopName.Trim(),
                Description = description.Trim(),
                Status = "Pending", // Chờ phê duyệt
                CreatedAt = DateTime.UtcNow,
            };

            return await _shopRepository.AddAsync(shop);
        }
    }
}
