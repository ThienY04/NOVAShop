using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab03.Models; // Thay bằng đúng namespace chứa DashboardViewModel
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NOVAShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Monday

            var orders = _context.Orders;

            var model = new DashboardViewModel
            {
                OrdersToday = await orders.CountAsync(o => o.OrderDate >= today),
                OrdersThisWeek = await orders.CountAsync(o => o.OrderDate >= startOfWeek),

                RevenueToday = await orders
                    .Where(o => o.OrderDate >= today)
                    .SumAsync(o => (decimal?)o.TotalPrice) ?? 0,

                RevenueThisWeek = await orders
                    .Where(o => o.OrderDate >= startOfWeek)
                    .SumAsync(o => (decimal?)o.TotalPrice) ?? 0,

                TotalRevenue = await orders.SumAsync(o => (decimal?)o.TotalPrice) ?? 0,

                LowStockCount = await _context.ProductVariants.CountAsync(p => p.Stock < 5),

                LowStockProducts = await _context.ProductVariants
                    .Include(p => p.Product)
                    .Where(p => p.Stock < 5)
                    .ToListAsync(),

                TopSellingProducts = await _context.OrderDetails
                    .GroupBy(d => d.Product.Name)
                    .Select(g => new TopProductViewModel
                    {
                        Name = g.Key,
                        QuantitySold = g.Sum(x => x.Quantity)
                    })
                    .OrderByDescending(x => x.QuantitySold)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }

    }
}