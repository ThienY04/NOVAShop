using lab03.Models;
using Microsoft.EntityFrameworkCore;

namespace lab03.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync();
            return await _context.Categories
            .Include(p => p.Products) // Include thông tin về category
            .ToListAsync();
        }
        // Lấy danh mục theo ID
        public async Task<Category> GetByIdAsync(int id)
        {
            // Lấy category kèm các sản phẩm (nếu cần)
            return await _context.Categories
                                 .Include(c => c.Products)  // Include sản phẩm liên quan (nếu cần)
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        // Xóa danh mục theo ID
        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories
        .Include(c => c.Products)
        .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                throw new Exception("Không tìm thấy danh mục.");
            }

            if (category.Products != null && category.Products.Any())
            {
                throw new Exception("Không thể xóa danh mục vì có sản phẩm liên quan.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

        }
    }
}
