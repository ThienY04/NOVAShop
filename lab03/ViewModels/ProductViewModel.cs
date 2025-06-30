using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace lab03.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; } // Th�m d�ng n�y ?? h? tr? c?p nh?t
        public required string Name { get; set; }

        public required decimal Price { get; set; }

        public required string Description { get; set; }

        public required int CategoryId { get; set; }

        public IFormFile? Image { get; set; }

        public List<string> Colors { get; set; } = new();

        public List<string> Sizes { get; set; } = new();

        public Dictionary<string, int> StockByVariant { get; set; } = new();
    }

}
