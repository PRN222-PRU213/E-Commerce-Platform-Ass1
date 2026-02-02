using E_Commerce_Platform_Ass1.Data.Database;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass1.Data.Repositories
{
    public class UserBehaviorRepository : IUserBehaviorRepository
    {
        private readonly ApplicationDbContext _context;

        public UserBehaviorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserBehavior behavior)
        {
            await _context.UserBehaviors.AddAsync(behavior);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserBehavior>> GetByUserIdAsync(Guid userId, int limit = 100)
        {
            return await _context.UserBehaviors
                .Where(ub => ub.UserId == userId)
                .OrderByDescending(ub => ub.CreatedAt)
                .Take(limit)
                .Include(ub => ub.Product)
                .Include(ub => ub.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserBehavior>> GetRecentByUserIdAsync(Guid userId, int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            return await _context.UserBehaviors
                .Where(ub => ub.UserId == userId && ub.CreatedAt >= cutoffDate)
                .OrderByDescending(ub => ub.CreatedAt)
                .Include(ub => ub.Product)
                    .ThenInclude(p => p!.Category)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, int>> GetTopViewedProductsAsync(Guid userId, int limit = 10)
        {
            return await _context.UserBehaviors
                .Where(ub => ub.UserId == userId && ub.ProductId != null && ub.ActionType == "View")
                .GroupBy(ub => ub.ProductId!.Value)
                .Select(g => new { ProductId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(limit)
                .ToDictionaryAsync(x => x.ProductId, x => x.Count);
        }

        public async Task<Dictionary<Guid, int>> GetTopCategoriesAsync(Guid userId, int limit = 5)
        {
            return await _context.UserBehaviors
                .Where(ub => ub.UserId == userId && ub.CategoryId != null)
                .GroupBy(ub => ub.CategoryId!.Value)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(limit)
                .ToDictionaryAsync(x => x.CategoryId, x => x.Count);
        }

        public async Task<IEnumerable<string>> GetRecentSearchQueriesAsync(Guid userId, int limit = 20)
        {
            return await _context.UserBehaviors
                .Where(ub => ub.UserId == userId && ub.ActionType == "Search" && ub.SearchQuery != null)
                .OrderByDescending(ub => ub.CreatedAt)
                .Take(limit)
                .Select(ub => ub.SearchQuery!)
                .Distinct()
                .ToListAsync();
        }
    }
}
