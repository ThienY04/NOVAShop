using lab03.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab03.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Danh sách đơn hàng đã bán
        public async Task<IActionResult> Sales()
        {
            var orders = await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // ✅ Chi tiết đơn hàng
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order); // Không cần kiểm tra vai trò nữa
        }
    }
}