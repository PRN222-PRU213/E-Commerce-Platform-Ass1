using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Models;
using E_Commerce_Platform_Ass1.Service.Services.IServices;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ Admin
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AdminService(
            IShopRepository shopRepository,
            IProductRepository productRepository,
            IProductVariantRepository productVariantRepository,
            IUserRepository userRepository,
            ICategoryRepository categoryRepository
        )
        {
            _shopRepository = shopRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
        }

        #region Shop Management

        public async Task<ServiceResult<List<ShopDto>>> GetAllShopsAsync()
        {
            var shops = await _shopRepository.GetAllAsync();
            var shopDtos = new List<ShopDto>();

            foreach (var shop in shops)
            {
                var owner = await _userRepository.GetByIdAsync(shop.UserId);
                var products = await _productRepository.GetByShopIdAsync(shop.Id);

                shopDtos.Add(
                    new ShopDto
                    {
                        Id = shop.Id,
                        UserId = shop.UserId,
                        ShopName = shop.ShopName,
                        Description = shop.Description,
                        Status = shop.Status,
                        CreatedAt = shop.CreatedAt,
                        OwnerName = owner?.Name,
                        OwnerEmail = owner?.Email,
                        ProductCount = products.Count(),
                    }
                );
            }

            return ServiceResult<List<ShopDto>>.Success(shopDtos);
        }

        public async Task<ServiceResult<List<ShopDto>>> GetShopsByStatusAsync(string status)
        {
            var shops = await _shopRepository.GetByStatus(status);
            var shopDtos = new List<ShopDto>();

            foreach (var shop in shops)
            {
                var owner = await _userRepository.GetByIdAsync(shop.UserId);
                var products = await _productRepository.GetByShopIdAsync(shop.Id);

                shopDtos.Add(
                    new ShopDto
                    {
                        Id = shop.Id,
                        UserId = shop.UserId,
                        ShopName = shop.ShopName,
                        Description = shop.Description,
                        Status = shop.Status,
                        CreatedAt = shop.CreatedAt,
                        OwnerName = owner?.Name,
                        OwnerEmail = owner?.Email,
                        ProductCount = products.Count(),
                    }
                );
            }

            return ServiceResult<List<ShopDto>>.Success(shopDtos);
        }

        public async Task<ServiceResult<ShopDetailDto>> GetShopDetailAsync(Guid shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return ServiceResult<ShopDetailDto>.Failure("Shop không tồn tại.");
            }

            var owner = await _userRepository.GetByIdAsync(shop.UserId);
            var products = await _productRepository.GetByShopIdAsync(shop.Id);

            var dto = new ShopDetailDto
            {
                Id = shop.Id,
                UserId = shop.UserId,
                ShopName = shop.ShopName,
                Description = shop.Description,
                Status = shop.Status,
                CreatedAt = shop.CreatedAt,
                OwnerName = owner?.Name,
                OwnerEmail = owner?.Email,
                ProductCount = products.Count(),
                Products = products
                    .Select(p => new ProductDto
                    {
                        Id = p.Id,
                        ShopId = p.ShopId,
                        CategoryId = p.CategoryId,
                        Name = p.Name,
                        Description = p.Description,
                        BasePrice = p.BasePrice,
                        Status = p.Status,
                        AvgRating = p.AvgRating,
                        ImageUrl = p.ImageUrl,
                        CreatedAt = p.CreatedAt,
                        ShopName = shop.ShopName,
                    })
                    .ToList(),
            };

            return ServiceResult<ShopDetailDto>.Success(dto);
        }

        public async Task<ServiceResult> ApproveShopAsync(Guid shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return ServiceResult.Failure("Shop không tồn tại.");
            }

            shop.Status = "Active";
            await _shopRepository.UpdateAsync(shop);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RejectShopAsync(Guid shopId, string? reason = null)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return ServiceResult.Failure("Shop không tồn tại.");
            }

            shop.Status = "Inactive";
            await _shopRepository.UpdateAsync(shop);

            return ServiceResult.Success();
        }

        #endregion

        #region Product Approval

        public async Task<ServiceResult<List<ProductDto>>> GetPendingProductsAsync()
        {
            return await GetProductsByStatusAsync("pending");
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllProductsAsync()
        {
            var allProducts = await _productRepository.GetAllAsync();
            // Admin chỉ thấy sản phẩm đã submit (không thấy draft)
            var products = allProducts.Where(p => p.Status != "draft");
            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                var shop = await _shopRepository.GetByIdAsync(product.ShopId);
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);

                productDtos.Add(
                    new ProductDto
                    {
                        Id = product.Id,
                        ShopId = product.ShopId,
                        CategoryId = product.CategoryId,
                        Name = product.Name,
                        Description = product.Description,
                        BasePrice = product.BasePrice,
                        Status = product.Status,
                        AvgRating = product.AvgRating,
                        ImageUrl = product.ImageUrl,
                        CreatedAt = product.CreatedAt,
                        ShopName = shop?.ShopName,
                        CategoryName = category?.Name,
                    }
                );
            }

            return ServiceResult<List<ProductDto>>.Success(productDtos);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetProductsByStatusAsync(string status)
        {
            var allProducts = await _productRepository.GetAllAsync();
            var products = allProducts.Where(p => p.Status == status);
            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                var shop = await _shopRepository.GetByIdAsync(product.ShopId);
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);

                productDtos.Add(
                    new ProductDto
                    {
                        Id = product.Id,
                        ShopId = product.ShopId,
                        CategoryId = product.CategoryId,
                        Name = product.Name,
                        Description = product.Description,
                        BasePrice = product.BasePrice,
                        Status = product.Status,
                        AvgRating = product.AvgRating,
                        ImageUrl = product.ImageUrl,
                        CreatedAt = product.CreatedAt,
                        ShopName = shop?.ShopName,
                        CategoryName = category?.Name,
                    }
                );
            }

            return ServiceResult<List<ProductDto>>.Success(productDtos);
        }

        public async Task<ServiceResult<ProductDetailDto>> GetProductForApprovalAsync(
            Guid productId
        )
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult<ProductDetailDto>.Failure("Sản phẩm không tồn tại.");
            }

            var variants = await _productVariantRepository.GetByProductIdAsync(productId);
            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            var shop = await _shopRepository.GetByIdAsync(product.ShopId);

            var dto = new ProductDetailDto
            {
                Id = product.Id,
                ShopId = product.ShopId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                Status = product.Status,
                AvgRating = product.AvgRating,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                CategoryName = category?.Name,
                ShopName = shop?.ShopName,
                Variants = variants
                    .Select(v => new ProductVariantDto
                    {
                        Id = v.Id,
                        ProductId = v.ProductId,
                        VariantName = v.VariantName,
                        Price = v.Price,
                        Size = v.Size,
                        Color = v.Color,
                        Stock = v.Stock,
                        Sku = v.Sku,
                        Status = v.Status,
                        ImageUrl = v.ImageUrl,
                    })
                    .ToList(),
            };

            return ServiceResult<ProductDetailDto>.Success(dto);
        }

        public async Task<ServiceResult> ApproveProductAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult.Failure("Sản phẩm không tồn tại.");
            }

            if (product.Status != "pending")
            {
                return ServiceResult.Failure(
                    "Chỉ có thể duyệt sản phẩm đang ở trạng thái chờ duyệt."
                );
            }

            product.Status = "active";
            await _productRepository.UpdateAsync(product);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RejectProductAsync(Guid productId, string? reason = null)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult.Failure("Sản phẩm không tồn tại.");
            }

            if (product.Status != "pending")
            {
                return ServiceResult.Failure(
                    "Chỉ có thể từ chối sản phẩm đang ở trạng thái chờ duyệt."
                );
            }

            product.Status = "rejected";
            await _productRepository.UpdateAsync(product);

            return ServiceResult.Success();
        }

        #endregion

        #region Statistics

        public async Task<AdminDashboardDto> GetDashboardStatisticsAsync()
        {
            var allShops = (await _shopRepository.GetAllAsync()).ToList();
            var allProducts = (await _productRepository.GetAllAsync()).ToList();
            var allUsers = (await _userRepository.GetAllAsync()).ToList();

            var dto = new AdminDashboardDto
            {
                // Shops
                TotalShops = allShops.Count,
                ActiveShops = allShops.Count(s => s.Status == "Active"),
                PendingShops = allShops.Count(s => s.Status == "Pending"),
                InactiveShops = allShops.Count(s => s.Status == "Inactive"),

                // Products
                TotalProducts = allProducts.Count,
                ActiveProducts = allProducts.Count(p => p.Status == "active"),
                PendingProducts = allProducts.Count(p => p.Status == "pending"),
                DraftProducts = allProducts.Count(p => p.Status == "draft"),
                RejectedProducts = allProducts.Count(p => p.Status == "rejected"),

                // Users
                TotalUsers = allUsers.Count,

                // Recent shops (last 5)
                RecentShops = new List<ShopDto>(),
                RecentPendingProducts = new List<ProductDto>(),
            };

            // Get recent shops
            var recentShops = allShops.OrderByDescending(s => s.CreatedAt).Take(5);
            foreach (var shop in recentShops)
            {
                var owner = await _userRepository.GetByIdAsync(shop.UserId);
                var products = await _productRepository.GetByShopIdAsync(shop.Id);
                dto.RecentShops.Add(
                    new ShopDto
                    {
                        Id = shop.Id,
                        UserId = shop.UserId,
                        ShopName = shop.ShopName,
                        Description = shop.Description,
                        Status = shop.Status,
                        CreatedAt = shop.CreatedAt,
                        OwnerName = owner?.Name,
                        OwnerEmail = owner?.Email,
                        ProductCount = products.Count(),
                    }
                );
            }

            // Get recent pending products (last 5)
            var recentPendingProducts = allProducts
                .Where(p => p.Status == "pending")
                .OrderByDescending(p => p.CreatedAt)
                .Take(5);

            foreach (var product in recentPendingProducts)
            {
                var shop = await _shopRepository.GetByIdAsync(product.ShopId);
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                dto.RecentPendingProducts.Add(
                    new ProductDto
                    {
                        Id = product.Id,
                        ShopId = product.ShopId,
                        CategoryId = product.CategoryId,
                        Name = product.Name,
                        Description = product.Description,
                        BasePrice = product.BasePrice,
                        Status = product.Status,
                        AvgRating = product.AvgRating,
                        ImageUrl = product.ImageUrl,
                        CreatedAt = product.CreatedAt,
                        ShopName = shop?.ShopName,
                        CategoryName = category?.Name,
                    }
                );
            }

            return dto;
        }

        #endregion
    }
}
