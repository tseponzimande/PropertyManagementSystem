namespace PropertyManagementSystem.Infrastructure.Repositories
{
    public class MaintenanceRequestRepository(PropertyManagementDbContext context) : Repository<MaintenanceRequest>(context), IMaintenanceRequestRepository
    {
        public async Task<IEnumerable<MaintenanceRequest>> GetMaintenanceRequestsByUnitAsync(Guid unitId)
        {
            return await _context.MaintenanceRequests
                .Include(m => m.Unit)
                .ThenInclude(u => u.Property)
                .Include(m => m.Tenant)
                .Where(m => m.UnitId == unitId)
                .OrderByDescending(m => m.Id)
                .ToListAsync();
        }
    }
}
