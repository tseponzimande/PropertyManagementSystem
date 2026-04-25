namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IMaintenanceService
    {
        Task<IEnumerable<MaintenanceRequestDto>> GetRequestsByUnitAsync(Guid unitId);
        Task<MaintenanceRequestDto?> CreateRequestAsync(MaintenanceRequestDto dto);
        Task<bool> UpdateRequestStatusAsync(Guid id, string status);
    }
}
