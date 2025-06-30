// Ánh xạ tên tiếng V thành mã màu hợp lệ
using System.Collections.Generic;

namespace lab03.Helpers
{
    public static class ColorHelper
    {
        private static readonly Dictionary<string, string> colorMap = new()
        {
            { "Đen", "#000000" },
            { "Trắng", "#FFFFFF" },
            { "Đỏ", "#FF0000" },
            { "Xanh dương", "#0000FF" },
            { "Xanh lá", "#008000" },
            { "Nâu", "#8B4513" },
            { "Hồng", "#FFC0CB" },
            { "Be", "#F5F5DC" },
            { "Vàng", "#FFFF00" },
            { "Tím", "#800080" },
            { "Xám", "#808080" },
            { "Cam", "#FFA500" },
            { "Xanh da trời", "#87CEEB" },
            { "Xanh ngọc", "#00FFFF" },
            { "Xanh rêu", "#556B2F" },
            { "Xanh pastel", "#B2FFFF" },
            { "Tím than", "#4B0082" },
            { "Cam đất", "#D2691E" },
            { "Vàng chanh", "#FFFACD" },
            { "Đỏ đô", "#8B0000" },
            { "Trắng ngà", "#FFFFF0" },
            { "Xám bạc", "#C0C0C0" },
            { "Xám tro", "#A9A9A9" },
            { "Hồng cánh sen", "#FFB6C1" },
            { "Hồng đất", "#BC8F8F" }
        };

        public static string ToColorCode(string colorName)
        {
            if (string.IsNullOrWhiteSpace(colorName)) return "#ccc";
            return colorMap.TryGetValue(colorName.Trim(), out var code) ? code : "#ccc"; // fallback
        }
    }
}