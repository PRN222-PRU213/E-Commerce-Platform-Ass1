using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass1.Service.Models;
using E_Commerce_Platform_Ass1.Service.Services.IServices;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ sản phẩm
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IShopRepository _shopRepository;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IShopRepository shopRepository
        )
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _shopRepository = shopRepository;
        }

        /// <summary>
        /// Tạo sản phẩm mới
        /// </summary>
        public async Task<ServiceResult<Guid>> CreateProductAsync(CreateProductDto dto)
        {
            // Validate Shop exists
            var shop = await _shopRepository.GetByIdAsync(dto.ShopId);
            if (shop == null)
            {
                return ServiceResult<Guid>.Failure("Shop không tồn tại.");
            }

            // Validate Category exists
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return ServiceResult<Guid>.Failure("Danh mục không tồn tại.");
            }

            // Validate Name
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return ServiceResult<Guid>.Failure("Tên sản phẩm không được để trống.");
            }

            // Validate Price
            if (dto.BasePrice < 0)
            {
                return ServiceResult<Guid>.Failure("Giá sản phẩm phải lớn hơn hoặc bằng 0.");
            }

            // Create product
            var product = new Product
            {
                Id = Guid.NewGuid(),
                ShopId = dto.ShopId,
                CategoryId = dto.CategoryId,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                BasePrice = dto.BasePrice,
                Status = "active", // Mặc định active khi tạo
                AvgRating = 0,
                ImageUrl = dto.ImageUrl?.Trim() ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
            };

            await _productRepository.AddAsync(product);

            return ServiceResult<Guid>.Success(product.Id);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm theo ShopId
        /// </summary>
        public async Task<ServiceResult<List<ProductDto>>> GetByShopIdAsync(Guid shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return ServiceResult<List<ProductDto>>.Failure("Shop không tồn tại.");
            }

            var products = await _productRepository.GetByShopIdAsync(shopId);
            var productDtos = products.Select(p => MapToDto(p, shop.ShopName)).ToList();

            return ServiceResult<List<ProductDto>>.Success(productDtos);
        }

        /// <summary>
        /// Lấy sản phẩm theo Id
        /// </summary>
        public async Task<ServiceResult<ProductDto>> GetByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult<ProductDto>.Failure("Sản phẩm không tồn tại.");
            }

            var dto = MapToDto(product);
            return ServiceResult<ProductDto>.Success(dto);
        }

        /// <summary>
        /// Lấy tất cả danh mục
        /// </summary>
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }).ToList();
        }

        // #region Private Helpers

        private static ProductDto MapToDto(Product product, string? shopName = null)
        {
            return new ProductDto
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
                ShopName = shopName,
            };
        }

        // #endregion
    }
}
