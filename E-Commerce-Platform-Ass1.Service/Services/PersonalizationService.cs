using System.Text.Json;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Services.IServices;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    public class PersonalizationService : IPersonalizationService
    {
        private readonly IUserBehaviorRepository _behaviorRepository;
        private readonly IUserPreferenceRepository _preferenceRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGeminiService _geminiService;

        public PersonalizationService(
            IUserBehaviorRepository behaviorRepository,
            IUserPreferenceRepository preferenceRepository,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IGeminiService geminiService)
        {
            _behaviorRepository = behaviorRepository;
            _preferenceRepository = preferenceRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _geminiService = geminiService;
        }

        public async Task<IEnumerable<PersonalizedProductDto>> GetPersonalizedProductsAsync(Guid userId, int limit = 12)
        {
            // 1. Lấy user preferences
            var preference = await _preferenceRepository.GetByUserIdAsync(userId);
            
            // 2. Nếu chưa có preferences, analyze trước
            if (preference == null || preference.LastAnalyzedAt == null)
            {
                await AnalyzeUserPreferencesAsync(userId);
                preference = await _preferenceRepository.GetByUserIdAsync(userId);
            }

            // 3. Lấy top categories từ behavior
            var topCategories = await _behaviorRepository.GetTopCategoriesAsync(userId, 5);
            var topProductIds = await _behaviorRepository.GetTopViewedProductsAsync(userId, 10);

            // 4. Query products based on preferences
            var products = await GetProductsByPreferencesAsync(preference, topCategories.Keys.ToList(), limit * 2);

            // 5. Score và rank products
            var scoredProducts = ScoreProducts(products, preference, topCategories, topProductIds);

            // 6. Return top products
            return scoredProducts
                .OrderByDescending(p => p.RelevanceScore)
                .Take(limit)
                .ToList();
        }

        public async Task<IEnumerable<PersonalizedProductDto>> GetRelatedProductsAsync(Guid userId, Guid currentProductId, int limit = 6)
        {
            var currentProduct = await _productRepository.GetByIdAsync(currentProductId);
            if (currentProduct == null)
                return Enumerable.Empty<PersonalizedProductDto>();

            // Lấy products cùng category
            var categoryProducts = await _productRepository.GetByCategoryIdAsync(currentProduct.CategoryId);
            
            // Lấy user preferences để personalize
            var preference = await _preferenceRepository.GetByUserIdAsync(userId);
            var topCategories = await _behaviorRepository.GetTopCategoriesAsync(userId, 5);

            var relatedProducts = categoryProducts
                .Where(p => p.Id != currentProductId && p.Status == "Active")
                .Select(p => new PersonalizedProductDto
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Price = p.BasePrice,
                    CategoryName = p.Category?.Name ?? string.Empty,
                    ShopId = p.ShopId,
                    ShopName = p.Shop?.ShopName ?? string.Empty,
                    RelevanceScore = CalculateRelevanceScore(p, preference, topCategories),
                    RecommendReason = $"Cùng danh mục {p.Category?.Name}"
                })
                .OrderByDescending(p => p.RelevanceScore)
                .Take(limit)
                .ToList();

            return relatedProducts;
        }

        public async Task<IEnumerable<PersonalizedProductDto>> GetFrequentlyBoughtTogetherAsync(Guid productId, int limit = 4)
        {
            var currentProduct = await _productRepository.GetByIdAsync(productId);
            if (currentProduct == null)
                return Enumerable.Empty<PersonalizedProductDto>();

            // Simplified: Get products from same category with similar price range
            var products = await _productRepository.GetByCategoryIdAsync(currentProduct.CategoryId);
            
            var priceMin = currentProduct.BasePrice * 0.5m;
            var priceMax = currentProduct.BasePrice * 2m;

            return products
                .Where(p => p.Id != productId && p.Status == "Active" && p.BasePrice >= priceMin && p.BasePrice <= priceMax)
                .Take(limit)
                .Select(p => new PersonalizedProductDto
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Price = p.BasePrice,
                    CategoryName = p.Category?.Name ?? string.Empty,
                    ShopId = p.ShopId,
                    ShopName = p.Shop?.ShopName ?? string.Empty,
                    RelevanceScore = 1.0,
                    RecommendReason = "Thường được mua cùng"
                })
                .ToList();
        }

        public async Task AnalyzeUserPreferencesAsync(Guid userId)
        {
            // 1. Lấy behavior data
            var behaviors = await _behaviorRepository.GetRecentByUserIdAsync(userId, 30);
            var searchQueries = await _behaviorRepository.GetRecentSearchQueriesAsync(userId, 20);

            if (!behaviors.Any())
            {
                // Không có data, tạo empty preference
                await _preferenceRepository.GetOrCreateAsync(userId);
                return;
            }

            // 2. Build prompt cho Gemini
            var prompt = BuildAnalysisPrompt(behaviors, searchQueries);

            // 3. Gọi Gemini API
            var analysis = await _geminiService.GenerateJsonContentAsync<AiPreferenceAnalysis>(prompt);

            // 4. Lưu preferences
            var preference = await _preferenceRepository.GetOrCreateAsync(userId);
            
            if (analysis != null)
            {
                preference.PreferredCategories = JsonSerializer.Serialize(analysis.PreferredCategories);
                preference.MinPriceRange = analysis.PriceRange?.Min;
                preference.MaxPriceRange = analysis.PriceRange?.Max;
                preference.StylePreferences = analysis.StylePreferences;
                preference.PreferredBrands = analysis.BrandAffinity != null 
                    ? JsonSerializer.Serialize(analysis.BrandAffinity) 
                    : null;
            }
            else
            {
                // Fallback: Use simple logic without AI
                await AnalyzeWithoutAiAsync(userId, preference, behaviors);
            }

            preference.LastAnalyzedAt = DateTime.UtcNow;
            await _preferenceRepository.UpdateAsync(preference);
        }

        public async Task<UserPreferenceDto?> GetUserPreferencesAsync(Guid userId)
        {
            var preference = await _preferenceRepository.GetByUserIdAsync(userId);
            if (preference == null) return null;

            return new UserPreferenceDto
            {
                PreferredCategories = !string.IsNullOrEmpty(preference.PreferredCategories)
                    ? JsonSerializer.Deserialize<List<string>>(preference.PreferredCategories) ?? new List<string>()
                    : new List<string>(),
                MinPriceRange = preference.MinPriceRange,
                MaxPriceRange = preference.MaxPriceRange,
                StylePreferences = preference.StylePreferences,
                PreferredBrands = !string.IsNullOrEmpty(preference.PreferredBrands)
                    ? JsonSerializer.Deserialize<List<string>>(preference.PreferredBrands)
                    : null,
                LastAnalyzedAt = preference.LastAnalyzedAt
            };
        }

        #region Private Methods

        private async Task<IEnumerable<Product>> GetProductsByPreferencesAsync(
            UserPreference? preference, 
            List<Guid> topCategoryIds, 
            int limit)
        {
            var allProducts = new List<Product>();

            // Lấy từ preferred categories (nếu có)
            if (topCategoryIds.Any())
            {
                foreach (var categoryId in topCategoryIds.Take(3))
                {
                    var products = await _productRepository.GetByCategoryIdAsync(categoryId);
                    allProducts.AddRange(products.Where(p => p.Status == "Active"));
                }
            }

            // LUÔN bổ sung thêm sản phẩm từ các category khác để đảm bảo có đủ sản phẩm hiển thị
            if (allProducts.Count < limit)
            {
                var categories = await _categoryRepository.GetAllAsync();
                var categoriesToFetch = categories
                    .Where(c => !topCategoryIds.Contains(c.Id))
                    .Take(6); // Lấy nhiều category hơn để đảm bảo có sản phẩm

                foreach (var category in categoriesToFetch)
                {
                    if (allProducts.Count >= limit) break;
                    var products = await _productRepository.GetByCategoryIdAsync(category.Id);
                    allProducts.AddRange(products.Where(p => p.Status == "Active"));
                }
            }

            // Filter theo price range nếu có
            if (preference?.MinPriceRange != null || preference?.MaxPriceRange != null)
            {
                var filtered = allProducts.Where(p =>
                    (preference.MinPriceRange == null || p.BasePrice >= preference.MinPriceRange) &&
                    (preference.MaxPriceRange == null || p.BasePrice <= preference.MaxPriceRange)
                ).ToList();
                
                // Nếu sau filter mà không còn sản phẩm, trả về list gốc (bỏ qua price filter)
                if (filtered.Any())
                    allProducts = filtered;
            }

            return allProducts.Distinct().Take(limit);
        }

        private List<PersonalizedProductDto> ScoreProducts(
            IEnumerable<Product> products,
            UserPreference? preference,
            Dictionary<Guid, int> topCategories,
            Dictionary<Guid, int> topProducts)
        {
            return products.Select(p =>
            {
                var score = CalculateRelevanceScore(p, preference, topCategories);
                
                // Boost nếu đã từng xem
                if (topProducts.ContainsKey(p.Id))
                    score += 0.2;

                var reason = DetermineRecommendReason(p, preference, topCategories);

                return new PersonalizedProductDto
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Price = p.BasePrice,
                    CategoryName = p.Category?.Name ?? string.Empty,
                    ShopId = p.ShopId,
                    ShopName = p.Shop?.ShopName ?? string.Empty,
                    RelevanceScore = score,
                    RecommendReason = reason
                };
            }).ToList();
        }

        private double CalculateRelevanceScore(Product product, UserPreference? preference, Dictionary<Guid, int> topCategories)
        {
            double score = 0.5; // Base score

            // Category match
            if (topCategories.ContainsKey(product.CategoryId))
            {
                var categoryRank = topCategories.Keys.ToList().IndexOf(product.CategoryId);
                score += 0.3 * (1.0 - categoryRank * 0.1);
            }

            // Price range match
            if (preference != null)
            {
                var inPriceRange = 
                    (preference.MinPriceRange == null || product.BasePrice >= preference.MinPriceRange) &&
                    (preference.MaxPriceRange == null || product.BasePrice <= preference.MaxPriceRange);
                
                if (inPriceRange) score += 0.2;
            }

            // Rating boost
            if (product.AvgRating >= 4.0m)
                score += 0.1;

            return Math.Min(score, 1.0);
        }

        private string DetermineRecommendReason(Product product, UserPreference? preference, Dictionary<Guid, int> topCategories)
        {
            if (topCategories.ContainsKey(product.CategoryId))
            {
                var categoryName = product.Category?.Name ?? "danh mục này";
                return $"Vì bạn quan tâm đến {categoryName}";
            }

            if (product.AvgRating >= 4.5m)
                return "Sản phẩm được đánh giá cao";

            return "Có thể bạn sẽ thích";
        }

        private string BuildAnalysisPrompt(IEnumerable<UserBehavior> behaviors, IEnumerable<string> searches)
        {
            var viewedProducts = behaviors
                .Where(b => b.ActionType == "View" && b.Product != null)
                .GroupBy(b => b.Product!.Name)
                .Select(g => $"- {g.Key} ({g.First().Product!.Category?.Name}, {g.First().Product!.BasePrice:N0} VNĐ) - {g.Count()} lần xem")
                .Take(10);

            var purchasedProducts = behaviors
                .Where(b => b.ActionType == "Purchase" && b.Product != null)
                .Select(b => $"- {b.Product!.Name} ({b.Product.Category?.Name})")
                .Distinct()
                .Take(5);

            var cartProducts = behaviors
                .Where(b => b.ActionType == "AddToCart" && b.Product != null)
                .Select(b => $"- {b.Product!.Name}")
                .Distinct()
                .Take(5);

            return $@"You are an e-commerce personalization AI. Analyze this Vietnamese user's shopping behavior and return their preferences as JSON.

USER BEHAVIOR DATA (last 30 days):

Viewed Products:
{string.Join("\n", viewedProducts)}

Added to Cart:
{string.Join("\n", cartProducts)}

Purchased:
{string.Join("\n", purchasedProducts)}

Search Queries:
{string.Join(", ", searches.Take(10))}

Analyze and return JSON with this exact structure:
{{
    ""preferredCategories"": [""Category1"", ""Category2""],
    ""priceRange"": {{ ""min"": 100000, ""max"": 5000000 }},
    ""stylePreferences"": ""Description of user's style preferences"",
    ""brandAffinity"": [""Brand1"", ""Brand2""],
    ""purchaseIntent"": ""Low/Medium/High"",
    ""recommendationStrategy"": ""Brief strategy for recommendations""
}}

Return ONLY valid JSON, no explanation.";
        }

        private async Task AnalyzeWithoutAiAsync(Guid userId, UserPreference preference, IEnumerable<UserBehavior> behaviors)
        {
            // Simple analysis without AI
            var topCategories = await _behaviorRepository.GetTopCategoriesAsync(userId, 5);
            var categories = await _categoryRepository.GetAllAsync();
            
            var categoryNames = categories
                .Where(c => topCategories.ContainsKey(c.Id))
                .Select(c => c.Name)
                .ToList();

            preference.PreferredCategories = JsonSerializer.Serialize(categoryNames);

            // Calculate price range from viewed products
            var prices = behaviors
                .Where(b => b.Product != null)
                .Select(b => b.Product!.BasePrice)
                .ToList();

            if (prices.Any())
            {
                preference.MinPriceRange = prices.Min() * 0.5m;
                preference.MaxPriceRange = prices.Max() * 1.5m;
            }
        }

        #endregion
    }
}
