namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string Method { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;

        public string TransactionCode { get; set; } = string.Empty;

        public DateTime PaidAt { get; set; }

        // Navigation property
        public Order Order { get; set; } = null!;

        public Payment(Guid orderId, decimal amount)
        {
            string datePart = DateTime.Now.ToString("ddMMyyyy");
            string guidPart = orderId.ToString("N").Substring(0, 8).ToUpper();

            Id = Guid.NewGuid();
            OrderId = orderId;
            Method = "Thanh toán bằng ví MoMo.";
            Amount = amount;
            Status = "Completed";
            TransactionCode = $"TXN-{datePart}-{guidPart}";
            PaidAt = DateTime.Now;
        }
    }
}
