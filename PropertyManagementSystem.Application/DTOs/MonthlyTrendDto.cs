namespace PropertyManagementSystem.Application.DTOs
{
    public class MonthlyTrendDto
    {
        public string Month { get; set; } = null!;
        public int Year { get; set; }
        public int MonthNumber { get; set; }
        public decimal Revenue { get; set; }
        public int PaymentCount { get; set; }
        public decimal AveragePaymentAmount { get; set; }
    }
}
