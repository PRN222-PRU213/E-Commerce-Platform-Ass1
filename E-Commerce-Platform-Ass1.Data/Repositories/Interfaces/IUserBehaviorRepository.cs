using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface IUserBehaviorRepository
    {
        Task AddAsync(UserBehavior behavior);
        Task<IEnumerable<UserBehavior>> GetByUserIdAsync(Guid userId, int limit = 100);
        Task<IEnumerable<UserBehavior>> GetRecentByUserIdAsync(Guid userId, int days = 30);
        Task<Dictionary<Guid, int>> GetTopViewedProductsAsync(Guid userId, int limit = 10);
        Task<Dictionary<Guid, int>> GetTopCategoriesAsync(Guid userId, int limit = 5);
        Task<IEnumerable<string>> GetRecentSearchQueriesAsync(Guid userId, int limit = 20);
    }
}
