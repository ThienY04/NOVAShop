using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace lab03.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }
        public string? Notes { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public string? DiscountCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? VoucherCode { get; set; }  // Mã giảm giá
        public string PaymentMethod { get; set; } = "COD";
        public string PaymentStatus { get; set; } = "Pending"; // Hoặc "Paid" khi admin xác nhận
        public string FullName { get; set; }
        public string PhoneNumber { get; set; } // đã có sẵn trong IdentityUser

    }

}
