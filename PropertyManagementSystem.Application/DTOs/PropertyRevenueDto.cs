namespace PropertyManagementSystem.Application.DTOs
{
    public class PropertyRevenueDto
    {
        public Guid PropertyId { get; set; }
        public string Address { get; set; } = null!;
        public decimal Revenue { get; set; }
        public int PaymentCount { get; set; }
    }
}
