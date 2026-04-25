namespace PropertyManagementSystem.Application.IRepository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetPaymentsByLeaseAsync(Guid leaseId);
    }
}
