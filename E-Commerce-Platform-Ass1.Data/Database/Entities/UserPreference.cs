namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    /// <summary>
    /// Entity lưu trữ sở thích người dùng được AI phân tích
    /// </summary>
    public class UserPreference
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        /// <summary>
        /// JSON chứa preferred categories: ["Electronics", "Fashion"]
        /// </summary>
        public string? PreferredCategories { get; set; }

        /// <summary>
        /// Khoảng giá ưa thích (min)
        /// </summary>
        public decimal? MinPriceRange { get; set; }

        /// <summary>
        /// Khoảng giá ưa thích (max)
        /// </summary>
        public decimal? MaxPriceRange { get; set; }

        /// <summary>
        /// AI-generated style preferences
        /// </summary>
        public string? StylePreferences { get; set; }

        /// <summary>
        /// Các brands ưa thích (JSON array)
        /// </summary>
        public string? PreferredBrands { get; set; }

        /// <summary>
        /// Lần cuối AI phân tích
        /// </summary>
        public DateTime? LastAnalyzedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}
