namespace PropertyManagementSystem.Application.DTOs
{
    public class MaintenanceBreakdownDto
    {
        public Guid PropertyId { get; set; }
        public string PropertyAddress { get; set; } = null!;
        public int TotalRequests { get; set; }
        public int OpenRequests { get; set; }
        public int ResolvedRequests { get; set; }
    }
}
