namespace PropertyManagementSystem.Application.DTOs
{
    #region Unit
    public class UnitDto
    {
        public Guid Id { get; set; }
        public string UnitNumber { get; set; } = null!;
        public decimal RentAmount { get; set; }
        public string Status { get; set; } = null!;
        public Guid PropertyId { get; set; }
    }

    #endregion
}
