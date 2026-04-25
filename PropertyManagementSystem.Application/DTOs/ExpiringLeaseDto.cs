namespace PropertyManagementSystem.Application.DTOs
{
    public class ExpiringLeaseDto
    {
        public Guid LeaseId { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = null!;
        public string PropertyAddress { get; set; } = null!;
        public string UnitNumber { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public int DaysUntilExpiry { get; set; }
    }
}
