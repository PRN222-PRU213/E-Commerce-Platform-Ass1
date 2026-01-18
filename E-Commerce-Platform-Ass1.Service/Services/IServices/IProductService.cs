using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Models;

namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    /// <summary>
    /// Service interface cho quản lý sản phẩm
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Tạo sản phẩm mới (draft)
        /// </summary>
        Task<List<Product>> GetAllProductAsync();

        Task<Product?> GetProductWithVariantsAsync(Guid productId);

        Task<ServiceResult<Guid>> CreateProductAsync(CreateProductDto dto);

        Task<ServiceResult<List<ProductDto>>> GetByShopIdAsync(Guid shopId);

        /// <summary>
        /// Lấy sản phẩm theo Id
        /// </summary>
        Task<ServiceResult<ProductDto>> GetByIdAsync(Guid productId);

        /// <summary>
        /// Lấy chi tiết sản phẩm bao gồm variants (dùng cho trang Edit)
        /// </summary>
        Task<ServiceResult<ProductDetailDto>> GetProductDetailAsync(Guid productId, Guid shopId);

        /// <summary>
        /// Submit sản phẩm để admin duyệt (chuyển từ draft sang pending)
        /// </summary>
        Task<ServiceResult> SubmitProductAsync(Guid productId, Guid shopId);

        /// <summary>
        /// Lấy tất cả danh mục active
        /// </summary>
        Task<List<CategoryDto>> GetAllCategoriesAsync();
    }
}