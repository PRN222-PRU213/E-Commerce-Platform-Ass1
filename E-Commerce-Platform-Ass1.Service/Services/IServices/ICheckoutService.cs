using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    public interface ICheckoutService
    {
        Task<Guid> CheckoutSuccessAsync(Guid userId, string shippingAddress);
    }
}
