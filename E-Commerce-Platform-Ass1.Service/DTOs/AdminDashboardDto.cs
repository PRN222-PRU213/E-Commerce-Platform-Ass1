namespace E_Commerce_Platform_Ass1.Service.DTOs
{
    /// <summary>
    /// DTO cho Dashboard Admin
    /// </summary>
    public class AdminDashboardDto
    {
        // Shops
        public int TotalShops { get; set; }
        public int ActiveShops { get; set; }
        public int PendingShops { get; set; }
        public int InactiveShops { get; set; }

        // Products
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int PendingProducts { get; set; }
        public int DraftProducts { get; set; }
        public int RejectedProducts { get; set; }

        // Users
        public int TotalUsers { get; set; }

        // Recent items
        public List<ShopDto> RecentShops { get; set; } = new();
        public List<ProductDto> RecentPendingProducts { get; set; } = new();
    }
}
