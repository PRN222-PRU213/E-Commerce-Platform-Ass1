using E_Commerce_Platform_Ass1.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var product = await _productService.GetProductWithVariantsAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
