namespace PropertyManagementSystem.Infrastructure.Repositories
{
    public class UnitRepository(PropertyManagementDbContext context) : Repository<Unit>(context), IUnitRepository
    {
        public async Task<IEnumerable<Unit>> GetUnitsByPropertyAsync(Guid propertyId)
        {
            return await _context.Units
                .Where(u => u.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Unit>> GetAvailableUnitsAsync()
        {
            return await _context.Units
                .Where(u => u.Status == UnitEnum.Available.ToString())
                .ToListAsync();
        }
    }
}
