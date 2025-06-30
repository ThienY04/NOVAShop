using Microsoft.AspNetCore.Mvc;

namespace lab03.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public string Size { get; set; }     // ➕ thêm
        public string Color { get; set; }    // ➕ thêm

        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
