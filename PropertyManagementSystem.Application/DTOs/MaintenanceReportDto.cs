namespace PropertyManagementSystem.Application.DTOs
{
    public class MaintenanceReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalRequests { get; set; }
        public int OpenRequests { get; set; }
        public int InProgressRequests { get; set; }
        public int ResolvedRequests { get; set; }
        public double AverageResolutionDays { get; set; }
        public List<MaintenanceBreakdownDto> Breakdown { get; set; } = new();
    }
}
