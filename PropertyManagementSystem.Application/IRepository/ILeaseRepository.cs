namespace PropertyManagementSystem.Application.IRepository
{
    public interface ILeaseRepository : IRepository<Lease>
    {
        Task<IEnumerable<Lease>> GetLeasesByUnitAsync(Guid unitId);
        Task<IEnumerable<Lease>> GetLeasesByTenantAsync(Guid tenantId);
    }
}
