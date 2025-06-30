using lab03.Models;
using lab03.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoriesAdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public CategoriesAdminController(IProductRepository productRepository,
        ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var category = await _categoryRepository.GetAllAsync();
            return View(category);
        }
        public async Task<IActionResult> Display(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
            await _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
            await _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index));

            }
            return View(category);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("DeleteConfirmed")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        // Kiểm tra có sản phẩm nào thuộc danh mục này không
        var products = await _productRepository.GetAllAsync();
        bool hasRelatedProducts = products.Any(p => p.CategoryId == id);

        if (hasRelatedProducts)
        {
            TempData["ErrorMessage"] = "Không thể xóa danh mục vì còn sản phẩm thuộc danh mục này.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await _categoryRepository.DeleteAsync(id);
            TempData["SuccessMessage"] = "Xóa danh mục thành công.";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa danh mục.";
        }

        return RedirectToAction(nameof(Index));
    }

}


