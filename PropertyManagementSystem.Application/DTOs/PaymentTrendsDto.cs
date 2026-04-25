namespace PropertyManagementSystem.Application.DTOs
{
    public class PaymentTrendsDto
    {
        public List<MonthlyTrendDto> MonthlyTrends { get; set; } = new();
        public decimal AverageMonthlyRevenue { get; set; }
        public decimal HighestMonthRevenue { get; set; }
        public decimal LowestMonthRevenue { get; set; }
        public string HighestRevenueMonth { get; set; } = null!;
        public double GrowthRate { get; set; }
    }
}
