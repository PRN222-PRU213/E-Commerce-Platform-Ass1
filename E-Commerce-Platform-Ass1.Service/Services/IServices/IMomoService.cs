using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Service.DTO.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;

namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    public interface IMomoService
    {
        Task<string> CreatePaymentAsync(long amount, string orderInfo);
    }
}
