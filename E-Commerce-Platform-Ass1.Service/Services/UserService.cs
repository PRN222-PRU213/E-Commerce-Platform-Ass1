using System;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Service.Utils;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<bool> RegisterAsync(string name, string email, string password)
        {
            var existing = await _userRepository.GetByEmailAsync(email);
            if (existing != null)
            {
                return false;
            }

            // Get default "User" role
            var userRole = await _roleRepository.GetByNameAsync("User");
            if (userRole == null)
            {
                throw new InvalidOperationException("Default 'Customer' role not found. Please seed roles first.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = PasswordHasher.HashPassword(password),
                RoleId = userRole.RoleId,
                Status = true,
                CreatedAt = DateTime.UtcNow,
            };

            await _userRepository.CreateAsync(user);
            return true;
        }

        public async Task<AuthenticatedUser?> ValidateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            // Verify password with backward compatibility (supports both hash and plain text)
            if (!PasswordHasher.VerifyPasswordWithBackwardCompat(password, user.PasswordHash))
            {
                return null;
            }

            // If password is stored as plain text, automatically hash it for security
            // This ensures migration happens gradually as users log in
            if (!PasswordHasher.IsBcryptHash(user.PasswordHash))
            {
                user.PasswordHash = PasswordHasher.HashPassword(password);
                await _userRepository.UpdateAsync(user);
            }

            return new AuthenticatedUser
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role?.Name ?? "Unknown",
            };
        }

        public async Task<AuthenticatedUser?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return new AuthenticatedUser
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role?.Name ?? "Unknown"
            };
        }
    }
}
