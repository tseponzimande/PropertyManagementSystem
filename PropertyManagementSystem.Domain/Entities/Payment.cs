namespace PropertyManagementSystem.Domain.Entities
{
    public class Payment : BaseEntity
    {
        #region Properties
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = PaymentEnum.Pending.ToString();
        #endregion

        #region Foreign Keys
        public Guid LeaseId { get; set; }
        #endregion

        #region Navigation Properties
        public Lease Lease { get; set; } = null!;
        #endregion
    }
}
