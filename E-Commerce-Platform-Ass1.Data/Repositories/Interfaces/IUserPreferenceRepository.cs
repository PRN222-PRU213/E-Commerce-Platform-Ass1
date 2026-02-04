using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface IUserPreferenceRepository
    {
        Task<UserPreference?> GetByUserIdAsync(Guid userId);
        Task AddAsync(UserPreference preference);
        Task UpdateAsync(UserPreference preference);
        Task<UserPreference> GetOrCreateAsync(Guid userId);
    }
}
