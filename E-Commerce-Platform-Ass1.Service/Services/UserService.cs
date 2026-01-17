using System;
using System.Threading.Tasks;
using BCrypt.Net;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.Services.IServices;

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
            var userRole = await _roleRepository.GetByNameAsync("Customer");
            if (userRole == null)
            {
                throw new InvalidOperationException("Default 'Customer' role not found. Please seed roles first.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = HashPassword(password),
                RoleId = userRole.RoleId,
                Status = true,
                CreatedAt = DateTime.UtcNow
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

            if (!VerifyPassword(password, user.PasswordHash))
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

        private static string HashPassword(string password)
        {
            // BCrypt tự sinh salt và lưu kèm trong hash
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}

