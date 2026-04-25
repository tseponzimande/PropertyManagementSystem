namespace PropertyManagementSystem.Application.DTOs
{
    public class MaintenanceStatsDto
    {
        public string Status { get; set; } = null!;
        public int Count { get; set; }
        public double AverageResolutionDays { get; set; }
    }
}
