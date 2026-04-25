namespace PropertyManagementSystem.Infrastructure.Repositories
{
    public class PropertyRepository(PropertyManagementDbContext context) : Repository<Property>(context), IPropertyRepository
    {
        public async Task<IEnumerable<Property>> GetPropertiesByOwnerAsync(Guid ownerId)
        {
            return await _context.Properties
                .Where(p => p.OwnerId == ownerId)  
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetApprovedPropertiesAsync()
        {
            return await _context.Properties
                .Where(p => p.Status == StatusEnum.Approved.ToString())
                .ToListAsync();
        }

    }
}
