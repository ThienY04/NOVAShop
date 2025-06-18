using lab03.Models;
using lab03.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace lab03.Controllers
{
    
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(string search)
        {
            var products = await _productRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.Trim();
                products = products.Where(p =>
                    p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (p.Category != null && p.Category.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            ViewBag.Search = search;
            return View(products);
        }
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

    }
}
