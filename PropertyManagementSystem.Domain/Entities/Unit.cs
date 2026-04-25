namespace PropertyManagementSystem.Domain.Entities
{
    public class Unit : BaseEntity
    {
        #region Properties
        public string UnitNumber { get; set; } = null!;
        public decimal RentAmount { get; set; }
        public string Status { get; set; } = UnitEnum.Available.ToString();

        public ICollection<Lease> Leases { get; set; } = new List<Lease>();

        #endregion

        #region Foreign Keys
        public Guid PropertyId { get; set; }
        #endregion

        #region Navigation Properties
        public Property Property { get; set; } = null!;
        #endregion
    }
}
