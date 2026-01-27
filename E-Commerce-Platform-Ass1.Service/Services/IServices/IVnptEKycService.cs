using E_Commerce_Platform_Ass1.Service.DTOs;
using Microsoft.AspNetCore.Http;

namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    public interface IVnptEKycService
    {
        Task<EKycResult> VerifyAsync(IFormFile front, IFormFile back, IFormFile selfie);
    }
}
