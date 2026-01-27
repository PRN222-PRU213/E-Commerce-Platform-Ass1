using Microsoft.AspNetCore.Http;

namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    public interface IEkycService
    {
        Task<bool> VerifyAndSaveAsync(Guid userId, IFormFile front, IFormFile back, IFormFile selfie);

        Task<bool> IsUserVerifiedAsync(Guid userId);
    }
}
