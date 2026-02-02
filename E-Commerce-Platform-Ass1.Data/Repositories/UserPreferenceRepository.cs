using E_Commerce_Platform_Ass1.Data.Database;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass1.Data.Repositories
{
    public class UserPreferenceRepository : IUserPreferenceRepository
    {
        private readonly ApplicationDbContext _context;

        public UserPreferenceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserPreference?> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserPreferences
                .FirstOrDefaultAsync(up => up.UserId == userId);
        }

        public async Task AddAsync(UserPreference preference)
        {
            await _context.UserPreferences.AddAsync(preference);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserPreference preference)
        {
            preference.UpdatedAt = DateTime.UtcNow;
            _context.UserPreferences.Update(preference);
            await _context.SaveChangesAsync();
        }

        public async Task<UserPreference> GetOrCreateAsync(Guid userId)
        {
            var existing = await GetByUserIdAsync(userId);
            if (existing != null)
            {
                return existing;
            }

            var preference = new UserPreference
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await AddAsync(preference);
            return preference;
        }
    }
}
