namespace E_Commerce_Platform_Ass1.Web.Models
{
    /// <summary>
    /// ViewModel cho Product Variant trong Detail page
    /// </summary>
    public class ProductVariantItemViewModel
    {
        public Guid Id { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsInStock => Stock > 0 && Status == "active";
    }

    /// <summary>
    /// ViewModel cho Review
    /// </summary>
    public class ProductReviewViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// ViewModel cho Home Index page - danh sách sản phẩm
    /// </summary>
    public class HomeIndexViewModel
    {
        public List<HomeProductItemViewModel> Products { get; set; } = new();
        
        /// <summary>
        /// Sản phẩm được AI cá nhân hóa cho user
        /// </summary>
        public List<PersonalizedProductViewModel> PersonalizedProducts { get; set; } = new();
    }

    /// <summary>
    /// ViewModel cho sản phẩm được AI cá nhân hóa
    /// </summary>
    public class PersonalizedProductViewModel
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string RecommendReason { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel cho Product item trong Home page
    /// </summary>
    public class HomeProductItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal AvgRating { get; set; }
        public string? ShopName { get; set; }
        public string? CategoryName { get; set; }
    }
}
