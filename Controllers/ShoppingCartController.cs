using lab03.Extensions;
using lab03.Models;
using lab03.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace lab03.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        private string GetCartKey()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return $"Cart_{User.Identity.Name}";
            }

            return $"Cart_Guest_{HttpContext.Session.Id}";
        }

        public IActionResult Index()
        {
            //để bắt buộc trình duyệt luôn tải lại trang
                /*  no-cache: trình duyệt phải xác thực lại với server trước khi dùng bản cache.
                    no-store: không lưu bất kỳ thông tin nào về response.
                    must-revalidate: phải xác thực lại với server.
                    Expires: 0: nội dung đã hết hạn ngay lập tức.*/
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(GetCartKey()) ?? new ShoppingCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity, string size, string color)
        {
            if (string.IsNullOrEmpty(size) || string.IsNullOrEmpty(color))
            {
                return Json(new { success = false, message = "Vui lòng chọn kích thước và màu sắc." });
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
            }

            // ✅ Tìm biến thể sản phẩm từ bảng ProductVariant
            var variant = _context.Set<ProductVariant>()
                .FirstOrDefault(v => v.ProductId == productId && v.Size == size && v.Color == color);

            if (variant == null)
            {
                return Json(new { success = false, message = "Không tìm thấy biến thể sản phẩm phù hợp." });
            }

            int stockAvailable = variant.Stock;

            // ✅ Lấy giỏ hàng hiện tại
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(GetCartKey()) ?? new ShoppingCart();

            // 🔎 Kiểm tra xem biến thể đã có trong giỏ chưa
            var existingItem = cart.Items.FirstOrDefault(i =>
                i.ProductId == productId &&
                i.Size == size &&
                i.Color == color);

            int quantityInCart = existingItem?.Quantity ?? 0;

            // ❌ Nếu vượt quá tồn kho
            if (quantityInCart + quantity > stockAvailable)
            {
                int availableToAdd = stockAvailable - quantityInCart;
                string message = availableToAdd > 0
                    ? $"Chỉ còn {availableToAdd} sản phẩm."
                    : "Bạn đã thêm hết số lượng còn lại trong kho cho sản phẩm này.";

                return Json(new { success = false, message = message });
            }

            // ✅ Hợp lệ → thêm vào giỏ
            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                Size = size,
                Color = color,
                ImageUrl = product.ImageUrl
            };

            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson(GetCartKey(), cart);

            // Cập nhật tổng số lượng cho biểu tượng giỏ hàng
            int totalQuantity = cart.Items.Sum(i => i.Quantity);
            // ✅ Cập nhật CartCount để layout có thể đọc
            HttpContext.Session.SetInt32("CartCount", totalQuantity);

            return Json(new
            {
                success = true,
                message = "Đã thêm vào giỏ.",
                cartCount = totalQuantity
            });
        }




        public IActionResult RemoveFromCart(int productId, string size, string color)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(GetCartKey()) ?? new ShoppingCart();

            cart.RemoveItem(productId, size, color);

            if (!cart.Items.Any())
            {
                // ✅ Nếu giỏ trống, xoá luôn khỏi session
                HttpContext.Session.Remove(GetCartKey());
                HttpContext.Session.SetInt32("CartCount", 0);
            }
            else
            {
                // ✅ Cập nhật lại giỏ và số lượng
                HttpContext.Session.SetObjectAsJson(GetCartKey(), cart);
                HttpContext.Session.SetInt32("CartCount", cart.Items.Sum(i => i.Quantity));
            }

            return RedirectToAction("Index");
        }



        public IActionResult Checkout()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(GetCartKey());
            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price,
                // Bạn có thể thêm Size/Color vào OrderDetail nếu DB hỗ trợ
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove(GetCartKey());

            return View("OrderCompleted", order.Id);
        }
        [HttpPost]
        public IActionResult UpdateCartItem(int productId, string size, string color, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(GetCartKey()) ?? new ShoppingCart();

            var existingItem = cart.Items.FirstOrDefault(i =>
                i.ProductId == productId &&
                i.Size == size &&
                i.Color == color);

            if (existingItem == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            // 🔍 Kiểm tra tồn kho
            var variant = _context.Set<ProductVariant>()
                .FirstOrDefault(v => v.ProductId == productId && v.Size == size && v.Color == color);

            if (variant == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
            }

            if (quantity > variant.Stock)
            {
                return Json(new
                {
                    success = false,
                    message = $"Chỉ còn {variant.Stock} sản phẩm trong kho cho sản phẩm này.",
                    availableStock = variant.Stock
                });
            }

            // ✅ Cập nhật giỏ hàng
            cart.UpdateItem(productId, size, color, quantity);
            HttpContext.Session.SetObjectAsJson(GetCartKey(), cart);
            HttpContext.Session.SetInt32("CartCount", cart.Items.Sum(i => i.Quantity));


            var item = cart.Items.First(i => i.ProductId == productId && i.Size == size && i.Color == color);
            return Json(new
            {
                success = true,
                totalItemPrice = item.Price * item.Quantity,
                cartTotal = cart.Items.Sum(i => i.Price * i.Quantity),
                cartCount = cart.Items.Sum(i => i.Quantity)
            });
        }



        private async Task<Product> GetProductFromDatabase(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }
    }
}
