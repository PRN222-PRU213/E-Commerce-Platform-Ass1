namespace E_Commerce_Platform_Ass1.Web.Models
{
    public class OrderHistoryViewModel
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public bool IsRefunded { get; set; }
    }
}
