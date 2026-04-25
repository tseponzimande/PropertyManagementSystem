namespace PropertyManagementSystem.Application.DTOs
{
    public class GrowthMetricsDto
    {
        public double UserGrowthRate { get; set; }
        public double PropertyGrowthRate { get; set; }
        public double RevenueGrowthRate { get; set; }
        public double OccupancyGrowthRate { get; set; }
        public int NewPropertiesThisMonth { get; set; }
        public int NewLeasesThisMonth { get; set; }
        public int NewTenantsThisMonth { get; set; }
        public Dictionary<string, double> MonthOverMonthGrowth { get; set; } = new();
    }
}
