namespace PropertyManagementSystem.Application.DTOs
{
    #region Lease

    public class LeaseDto
    {
        public Guid Id { get; set; }
        public Guid UnitId { get; set; }
        public Guid TenantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal RentAmount { get; set; }
        public string Status { get; set; } = null!;
    }

    #endregion
}
