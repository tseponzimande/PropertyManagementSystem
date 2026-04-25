namespace PropertyManagementSystem.Application.DTOs
{
    public class MaintenanceTrendDto
    {
        public string Month { get; set; } = null!;
        public int TotalRequests { get; set; }
        public int OpenRequests { get; set; }
        public int ResolvedRequests { get; set; }
        public double AverageResolutionDays { get; set; }
    }
}
