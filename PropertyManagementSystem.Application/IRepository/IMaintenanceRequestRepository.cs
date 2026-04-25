namespace PropertyManagementSystem.Application.IRepository
{
    public interface IMaintenanceRequestRepository : IRepository<MaintenanceRequest>
    {
        Task<IEnumerable<MaintenanceRequest>> GetMaintenanceRequestsByUnitAsync(Guid unitId);
    }
}
