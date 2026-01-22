using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.Services.IServices;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    public class RefundService : IRefundService
    {
        private readonly IRefundRepository _refundRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMomoApi _momoApi;
        private readonly IOrderRepository _orderRepository;

        public RefundService(IRefundRepository refundRepository, IPaymentRepository paymentRepository, IMomoApi momoApi, IOrderRepository orderRepository)
        {
            _refundRepository = refundRepository;
            _paymentRepository = paymentRepository;
            _momoApi = momoApi;
            _orderRepository = orderRepository;
        }
        public async Task RefundAsync(Guid orderId, decimal amount, string reason)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            if (payment == null || payment.Status == "Refunded")
            {
                throw new Exception("Payment not refundable");
            }

            var requestId = Guid.NewGuid().ToString();

            var isExisted = await _refundRepository.ExistsRequestIdAsync(requestId);
            if (isExisted)
            {
                throw new Exception("Duplicate refund request");
            }

            var momoResult = await _momoApi.RefundAsync(requestId, amount, reason);
            if (momoResult.ResultCode != 0)
                throw new Exception("MoMo refund failed");

            var refund = new Refund
            {
                Id = Guid.NewGuid(),
                PaymentId = payment.Id,
                RequestId = requestId,
                RefundAmount = amount,
                Reason = reason,
                Status = "Success",
                CreatedAt = DateTime.Now
            };

            await _refundRepository.AddAsync(refund);

            payment.Status = "Refunded";
            await _paymentRepository.UpdateAsync(payment);

            order.Status = "CANCELLED";
            await _orderRepository.UpdateAsync(order);
        }
    }
}
