using System;
using System.ComponentModel.DataAnnotations;
namespace lab03.Models
{
    public class Discount
    {
        public int Id { get; set; }

        public string Code { get; set; }  // Ví dụ: "SALE10"

        public decimal Value { get; set; }  // Phần trăm giảm, ví dụ: 10 nghĩa là giảm 10%

        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }


}
