namespace PropertyManagementSystem.Application.DTOs
{
    public class TenantDetailDto
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PropertyAddress { get; set; } = null!;
        public string UnitNumber { get; set; } = null!;
        public DateTime LeaseStartDate { get; set; }
        public DateTime LeaseEndDate { get; set; }
        public string LeaseStatus { get; set; } = null!;
        public decimal RentAmount { get; set; }
    }
}
