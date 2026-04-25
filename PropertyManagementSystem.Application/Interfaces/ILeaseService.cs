namespace PropertyManagementSystem.Application.Interfaces
{
    public interface ILeaseService
    {
        Task<IEnumerable<LeaseDto>> GetAllLeasesAsync();
        Task<LeaseDto?> GetLeaseByIdAsync(Guid id);
        Task<IEnumerable<LeaseDto>> GetLeasesByUnitAsync(Guid unitId);
        Task<IEnumerable<LeaseDto>> GetLeasesByTenantAsync(Guid tenantId);
        Task<LeaseDto?> CreateLeaseAsync(LeaseDto dto);
        Task<bool> UpdateLeaseAsync(LeaseDto dto);
        Task<bool> DeleteLeaseAsync(Guid id);
        Task<bool> TerminateLeaseAsync(Guid id);
    }
}
