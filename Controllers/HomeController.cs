using lab03.Models;
using lab03.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace lab03.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;

        public HomeController(IProductRepository productRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }
        public async Task<IActionResult> Index(string? search, int? categoryId, int? minPrice, int? maxPrice, string? sort)
        {
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(search));
            }

            // Lọc danh mục
            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }

            // Lọc khoảng giá
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice);
            }

            // Sắp xếp
            productsQuery = sort switch
            {
                "name_asc" => productsQuery.OrderBy(p => p.Name),
                "name_desc" => productsQuery.OrderByDescending(p => p.Name),
                "price_asc" => productsQuery.OrderBy(p => p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                _ => productsQuery.OrderByDescending(p => p.Id) // mặc định: mới nhất
            };

            // Lấy danh sách danh mục cho dropdown
            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(await productsQuery.ToListAsync());
        }


        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        public IActionResult Branches()
        {
            var branches = new List<BranchView>
            {
                new BranchView
                {
                    Title = "Chi nhánh Quận Bình Thạnh",
                    Address = "Vạn Kiếp, phường 1, Quận Bình Thạnh, TP.HCM",
                    PhoneNumber = "0763 972 150",
                    GoogleMapsEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3919.1550554353457!2d106.69097007469749!3d10.799433889350745!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317528c5dd7cf211%3A0x2d36515e500e098a!2zxJAuIFbhuqFuIEtp4bq_cCwgcGjGsOG7nW5nIDEsIELDrG5oIFRo4bqhbmgsIEjhu5MgQ2jDrSBNaW5oLCBWaeG7h3QgTmFt!5e0!3m2!1svi!2s!4v1750268978204!5m2!1svi!2s"
                },
                //new BranchView
                //{
                //    Title = "Chi nhánh Quận 1",
                //    Address = "30 Mạc Đĩnh Chi,Đa Kao, Quận 1, TP.HCM",
                //    PhoneNumber = "0333787207",
                //    GoogleMapsEmbedUrl = "https://www.google.com/maps/embed?pb=!1m14!1m12!1m3!1d473.15230494631317!2d106.69821469683755!3d10.786748296668828!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!5e0!3m2!1svi!2s!4v1750270422847!5m2!1svi!2s"
                //}
            };
            return View(branches);
        }

    }
}
