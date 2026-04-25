namespace PropertyManagementSystem.Application.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalProperties { get; set; }
        public int TotalUnits { get; set; }
        public int AvailableUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public int TotalLeases { get; set; }
        public int ActiveLeases { get; set; }
        public int ExpiredLeases { get; set; }
        public int TotalTenants { get; set; }
        public int TotalOwners { get; set; }
        public int PendingMaintenanceRequests { get; set; }
        public int InProgressMaintenanceRequests { get; set; }
        public int ResolvedMaintenanceRequests { get; set; }
        public decimal TotalRevenueThisMonth { get; set; }
        public decimal TotalRevenueThisYear { get; set; }
        public int PendingPropertyApprovals { get; set; }
        public int UnreadMessages { get; set; }
        public int UnreadNotifications { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
