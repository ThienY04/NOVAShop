using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using lab03.Models;
using lab03.Repositories;
using lab03.ViewModels;
using Microsoft.EntityFrameworkCore; // Thay thế bằng namespace thực tếcủa bạn

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductAdminController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ApplicationDbContext _context;
    public ProductAdminController(IProductRepository productRepository, ICategoryRepository categoryRepository, ApplicationDbContext context)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _context = context;
    }
    // Hiển thị danh sách sản phẩm
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Index(string? searchName, int? categoryId)
    {
        var products = await _productRepository.GetAllWithVariantsAsync();
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");

        if (!string.IsNullOrEmpty(searchName))
        {
            products = products.Where(p => p.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (categoryId.HasValue && categoryId.Value != 0)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
        }

        return View(products);
    }


    // Hiển thị form thêm sản phẩm mới
    public async Task<IActionResult> Add()
    {
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");

        return View();
    }
    // Xử lý thêm sản phẩm mới
    [HttpPost]
    public async Task<IActionResult> Add(ProductViewModel model)
    {
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");

        if (!ModelState.IsValid)
            return View(model);

        // Lưu ảnh
        string? savedImage = null;
        if (model.Image != null)
        {
            savedImage = await SaveImage(model.Image);
        }

        // Tạo Product chính
        var product = new Product
        {
            Name = model.Name,
            Price = model.Price,
            Description = model.Description,
            ImageUrl = savedImage,
            CategoryId = model.CategoryId
        };

        await _productRepository.AddAsync(product);

        // Duyệt biến thể (đúng với form bạn làm)
        int count = Math.Min(
            model.Colors?.Count ?? 0,
            model.Sizes?.Count ?? 0
        );

        for (int i = 0; i < count; i++)
        {
            string color = model.Colors![i];
            string size = model.Sizes![i];
            string key = $"{i}";
            if (model.StockByVariant.TryGetValue(key, out int stock))
            {
                var variant = new ProductVariant
                {
                    ProductId = product.Id,
                    Color = color,
                    Size = size,
                    Stock = stock
                };
                _context.ProductVariants.Add(variant);
            }
        }


        await _context.SaveChangesAsync();

        // Gán lại danh sách biến thể cho product
        product.Variants = await _context.ProductVariants
            .Where(v => v.ProductId == product.Id)
            .ToListAsync();

        return RedirectToAction(nameof(Index));

    }


    // Viết thêm hàm SaveImage (tham khảo bài 02)
    private async Task<string> SaveImage(IFormFile image)
    {
        //Thay đổi đường dẫn theo cấu hình của bạn
        var savePath = Path.Combine("wwwroot/images", image.FileName);
        using (var fileStream = new FileStream(savePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }
        return image.FileName; // Trả về đường dẫn tương đối
    }
    // Hiển thị thông tin chi tiết sản phẩm
    public async Task<IActionResult> Display(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants) // <== Thêm dòng này
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // Hiển thị form cập nhật sản phẩm
    public async Task<IActionResult> Update(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name",
        product.CategoryId);
        return View(product);
    }
    // Xử lý cập nhật sản phẩm
    [HttpPost]
    public async Task<IActionResult> Update(int id, Product product, FormFile imageUrl)
    {
        ModelState.Remove("ImageUrl");
        if (id != product.Id)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            var existingProduct = await
            _productRepository.GetByIdAsync(id);
            if (imageUrl == null)
            {
                product.ImageUrl = existingProduct.ImageUrl;
            }
            else
            {
                // Lưu hình ảnh mới
                product.ImageUrl = await SaveImage(imageUrl);
            }
            // Cập nhật các thông tin khác của sản phẩm
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.ImageUrl = product.ImageUrl;
            await _productRepository.UpdateAsync(existingProduct);
            return RedirectToAction(nameof(Index));
        }
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(product);
    }
    // // Hiển thị form xác nhận xóa sản phẩm
    // public async Task<IActionResult> Delete(int id)
    // {
    //     var product = await _productRepository.GetByIdAsync(id);
    //     if (product == null)
    //     {
    //         return NotFound();
    //     }
    //     return View(product);
    // }
    // Xử lý xóa sản phẩm
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _productRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }


    //
    public async Task<IActionResult> Detail(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        return View(product);
    }
}