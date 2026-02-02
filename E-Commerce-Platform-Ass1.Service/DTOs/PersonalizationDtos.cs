namespace E_Commerce_Platform_Ass1.Service.DTOs
{
    public class UserBehaviorDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string? SearchQuery { get; set; }
        public decimal? ProductPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PersonalizedProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public double RelevanceScore { get; set; }
        public string RecommendReason { get; set; } = string.Empty;
    }

    public class UserPreferenceDto
    {
        public List<string> PreferredCategories { get; set; } = new();
        public decimal? MinPriceRange { get; set; }
        public decimal? MaxPriceRange { get; set; }
        public string? StylePreferences { get; set; }
        public List<string>? PreferredBrands { get; set; }
        public DateTime? LastAnalyzedAt { get; set; }
    }

    /// <summary>
    /// Response từ Gemini AI khi phân tích user behavior
    /// </summary>
    public class AiPreferenceAnalysis
    {
        public List<string> PreferredCategories { get; set; } = new();
        public PriceRangeDto? PriceRange { get; set; }
        public string? StylePreferences { get; set; }
        public List<string>? BrandAffinity { get; set; }
        public string? PurchaseIntent { get; set; }
        public string? RecommendationStrategy { get; set; }
    }

    public class PriceRangeDto
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }
}
