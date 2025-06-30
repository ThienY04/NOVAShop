using lab03.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NOVAShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // ✅ Chỉ Admin mới truy cập được
    public class DiscountAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscountAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Danh sách mã giảm giá
        public async Task<IActionResult> Index()
        {
            var discounts = await _context.Discounts.ToListAsync();
            return View(discounts);
        }

        // ✅ Form tạo mới mã giảm giá
        public IActionResult Create()
        {
            return View();
        }

        // ✅ Lưu mã mới vào DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discount discount)
        {
            if (ModelState.IsValid)
            {
                _context.Discounts.Add(discount);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Tạo mã giảm giá thành công.";
                return RedirectToAction(nameof(Index));
            }
            return View(discount);
        }

        // ✅ Form chỉnh sửa mã giảm giá
        public async Task<IActionResult> Edit(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }

        // ✅ Cập nhật mã vào DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Discount discount)
        {
            if (id != discount.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(discount);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Cập nhật mã giảm giá thành công.";
                return RedirectToAction(nameof(Index));
            }
            return View(discount);
        }

        // ✅ Xác nhận xoá
        public async Task<IActionResult> Delete(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null) return NotFound();

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Đã xoá mã giảm giá.";
            return RedirectToAction(nameof(Index));
        }
        // ✅ Xem chi tiết mã giảm giá
        public async Task<IActionResult> Details(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }

    }
}
