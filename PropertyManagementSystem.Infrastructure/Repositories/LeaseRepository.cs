namespace PropertyManagementSystem.Infrastructure.Repositories
{
    public class LeaseRepository : Repository<Lease>, ILeaseRepository
    {
        public LeaseRepository(PropertyManagementDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Lease>> GetLeasesByUnitAsync(Guid unitId)
        {
            return await _context.Leases
                .Include(l => l.Unit)
                .Include(l => l.Tenant)
                .Where(l => l.UnitId == unitId)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lease>> GetLeasesByTenantAsync(Guid tenantId)
        {
            return await _context.Leases
                .Include(l => l.Unit)
                    .ThenInclude(u => u.Property)
                .Where(l => l.TenantId == tenantId)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }
    }
}