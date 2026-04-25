namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetPaymentsByLeaseAsync(Guid leaseId);
        Task<PaymentDto?> GetPaymentByIdAsync(Guid id);
        Task<PaymentDto?> CreatePaymentAsync(PaymentDto dto);
    }
}
