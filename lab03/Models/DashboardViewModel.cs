using lab03.Models;

public class DashboardViewModel
{
    public int OrdersToday { get; set; }
    public int OrdersThisWeek { get; set; }
    public decimal RevenueToday { get; set; }          // ✅ Doanh thu hôm nay
    public decimal RevenueThisWeek { get; set; }       // ✅ Doanh thu tuần 
    public decimal TotalRevenue { get; set; }
    public int LowStockCount { get; set; }

    public List<TopProductViewModel> TopSellingProducts { get; set; } = new();
    public List<ProductVariant> LowStockProducts { get; set; }

}

public class TopProductViewModel
{
    public string Name { get; set; }
    public int QuantitySold { get; set; }
}