namespace PropertyManagementSystem.Domain.Entities
{
    public class Lease : BaseEntity
    {
        #region Properties
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal RentAmount { get; set; }
        public string Status { get; set; } = LeaseEnum.Active.ToString();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        #endregion

        #region Foreign Keys

        public Guid UnitId { get; set; }
        public Guid TenantId { get; set; }
        #endregion

        #region Navigation Properties

        public Unit Unit { get; set; } = null!;
        public User Tenant { get; set; } = null!;
        #endregion
    }
}
