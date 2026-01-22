using System;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Service.DTOs;

namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    public interface IShopService
    {
        Task<Shop?> GetShopByUserIdAsync(Guid userId);
        Task<Shop?> GetShopByIdAsync(Guid shopId);
        Task<bool> UserHasShopAsync(Guid userId);
        Task<bool> ShopNameExistsAsync(string shopName);
        Task<Shop> RegisterShopAsync(Guid userId, string shopName, string description);
        
        /// <summary>
        /// Lấy thống kê tổng quan của Shop
        /// </summary>
        Task<ShopStatisticsDto> GetShopStatisticsAsync(Guid shopId);
    }
}
