namespace PropertyManagementSystem.Application.DTOs
{
    #region Payment

    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid LeaseId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = null!;
    }

    #endregion
}
