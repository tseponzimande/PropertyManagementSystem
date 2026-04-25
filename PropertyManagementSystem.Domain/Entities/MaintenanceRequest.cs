namespace PropertyManagementSystem.Domain.Entities
{
    public class MaintenanceRequest : BaseEntity
    {
        #region Scalar Properties
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = MaintenanceRequestEnum.Open.ToString();
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
