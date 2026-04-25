namespace PropertyManagementSystem.Application.IRepository
{
    public interface IPropertyRepository : IRepository<Property>
    {
        Task<IEnumerable<Property>> GetPropertiesByOwnerAsync(Guid ownerId);
        Task<IEnumerable<Property>> GetApprovedPropertiesAsync();
    }
}
