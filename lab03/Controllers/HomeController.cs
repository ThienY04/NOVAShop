using lab03.Models;
using lab03.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace lab03.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;

        public HomeController(IProductRepository productRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        // Trang ch? - hi?n th? s?n ph?m, có tìm ki?m, l?c, s?p x?p
        public async Task<IActionResult> Index(string? search, int? categoryId, int? minPrice, int? maxPrice, string? sort)
        {
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(search));
            }

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice);
            }

            productsQuery = sort switch
            {
                "name_asc" => productsQuery.OrderBy(p => p.Name),
                "name_desc" => productsQuery.OrderByDescending(p => p.Name),
                "price_asc" => productsQuery.OrderBy(p => p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                _ => productsQuery.OrderByDescending(p => p.Id) // M?c ??nh: m?i nh?t
            };

            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(await productsQuery.ToListAsync());
        }

        // Trang chính sách riêng t?
        public IActionResult Privacy()
        {
            return View();
        }

        // Trang l?i
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Trang danh sách chi nhánh
        public IActionResult Branches()
        {
            var branches = new List<BranchView>
            {
                new BranchView
                {
                    Title = "Chi nhánh Qu?n Bình Th?nh",
                    Address = "V?n Ki?p, ph??ng 1, Qu?n Bình Th?nh, TP.HCM",
                    PhoneNumber = "0763 972 150",
                    GoogleMapsEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3919.1550554353457!2d106.69097007469749!3d10.799433889350745!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317528c5dd7cf211%3A0x2d36515e500e098a!2zxJAuIFbhuqFuIEtp4bq_cCwgcGjGsOG7nW5nIDEsIELDrG5oIFRo4bqhbmgsIEjhu5MgQ2jDrSBNaW5oLCBWaeG7h3QgTmFt!5e0!3m2!1svi!2s!4v1750268978204!5m2!1svi!2s"
                }
                // Có th? thêm chi nhánh khác ? ?ây n?u c?n
            };

            return View(branches);
        }
    }
}
