namespace PropertyManagementSystem.Application.DTOs
{
    public class MaintenanceTrendsDto
    {
        public List<MaintenanceTrendDto> MonthlyTrends { get; set; } = new();
        public double AverageResolutionTime { get; set; }
        public int TotalRequestsThisMonth { get; set; }
        public int ResolvedThisMonth { get; set; }
        public double ResolutionRate { get; set; }
    }
}
