namespace PropertyManagementSystem.Application.DTOs
{
    public class RevenueStatsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public decimal LastMonthRevenue { get; set; }
        public decimal ThisYearRevenue { get; set; }
        public decimal AverageMonthlyRevenue { get; set; }
        public List<MonthlyRevenueDto> MonthlyBreakdown { get; set; } = new();
    }
}
