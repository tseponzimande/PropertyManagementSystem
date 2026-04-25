namespace PropertyManagementSystem.Application.DTOs
{
    public class LeaseExpiryReportDto
    {
        public int DaysAhead { get; set; }
        public int ExpiringLeases { get; set; }
        public List<ExpiringLeaseDto> Leases { get; set; } = new();
    }
}
