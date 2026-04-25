namespace PropertyManagementSystem.Application.DTOs
{
    public class SystemAnalyticsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int TotalProperties { get; set; }
        public int TotalUnits { get; set; }
        public decimal AverageOccupancyRate { get; set; }
        public decimal TotalRevenueThisMonth { get; set; }
        public decimal TotalRevenueThisYear { get; set; }
        public int ActiveLeases { get; set; }
        public int PendingMaintenanceRequests { get; set; }
        public double AverageMaintenanceResolutionDays { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
