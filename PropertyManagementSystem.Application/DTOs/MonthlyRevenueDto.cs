namespace PropertyManagementSystem.Application.DTOs
{
    public class MonthlyRevenueDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = null!;
        public decimal Revenue { get; set; }
        public int PaymentCount { get; set; }
    }
}
