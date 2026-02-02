namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    /// <summary>
    /// Entity lưu trữ hành vi người dùng để phân tích và cá nhân hóa
    /// </summary>
    public class UserBehavior
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// ActionType: View, Search, AddToCart, Purchase, Wishlist
        /// </summary>
        public string ActionType { get; set; } = string.Empty;

        /// <summary>
        /// SearchQuery nếu ActionType = Search
        /// </summary>
        public string? SearchQuery { get; set; }

        /// <summary>
        /// Metadata bổ sung (JSON) - ví dụ: quantity, price at time
        /// </summary>
        public string? Metadata { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Product? Product { get; set; }
        public Category? Category { get; set; }
    }
}
