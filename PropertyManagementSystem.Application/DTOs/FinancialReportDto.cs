namespace PropertyManagementSystem.Application.DTOs
{
    public class FinancialReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalPending { get; set; }
        public int TotalPayments { get; set; }
        public int PendingPayments { get; set; }
        public List<PaymentBreakdownDto> PaymentBreakdown { get; set; } = new();
        public List<PropertyRevenueDto> PropertyRevenue { get; set; } = new();
    }
}
