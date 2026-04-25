namespace PropertyManagementSystem.Application.DTOs
{
    public class TenantReportDto
    {
        public int TotalTenants { get; set; }
        public int ActiveTenants { get; set; }
        public int TenantsWithExpiredLeases { get; set; }
        public List<TenantDetailDto> Tenants { get; set; } = new();
    }
}
