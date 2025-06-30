using lab03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
namespace lab03.Controllers
{
    public class DiscountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Cho khách hàng xem danh sách mã hợp lệ
        public async Task<IActionResult> PublicList()
        {
            var today = DateTime.Now;

            var discounts = await _context.Discounts
                .Where(d => d.IsActive && d.ExpiryDate >= today)
                .ToListAsync();

            return View(discounts);
        }

        // ✅ Khách hàng áp dụng mã sẽ được xử lý ở ShoppingCartController
    }
}
