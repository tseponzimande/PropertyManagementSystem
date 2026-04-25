namespace PropertyManagementSystem.Application.IRepository
{
    public interface IUnitRepository : IRepository<Unit>
    {
        Task<IEnumerable<Unit>> GetUnitsByPropertyAsync(Guid propertyId);
        Task<IEnumerable<Unit>> GetAvailableUnitsAsync();
    }
}
