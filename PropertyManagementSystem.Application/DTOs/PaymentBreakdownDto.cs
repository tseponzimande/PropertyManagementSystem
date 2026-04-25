namespace PropertyManagementSystem.Application.DTOs
{
    public class PaymentBreakdownDto
    {
        public string Month { get; set; } = null!;
        public decimal Amount { get; set; }
        public int PaymentCount { get; set; }
    }
}
