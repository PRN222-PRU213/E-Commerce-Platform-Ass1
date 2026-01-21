using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    public interface ICheckoutService
    {
        Task<Order> CheckoutSuccessAsync(Guid userId, string shippingAddress);
    }
}
