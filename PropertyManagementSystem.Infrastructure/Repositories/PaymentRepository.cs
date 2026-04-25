namespace PropertyManagementSystem.Infrastructure.Repositories
{
    public class PaymentRepository(PropertyManagementDbContext context) : Repository<Payment>(context), IPaymentRepository
    {
        public async Task<IEnumerable<Payment>> GetPaymentsByLeaseAsync(Guid leaseId)
        {
            return await _context.Payments
                .Include(p => p.Lease)
                    .ThenInclude(l => l.Unit)
                .Where(p => p.LeaseId == leaseId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }
    }
}
