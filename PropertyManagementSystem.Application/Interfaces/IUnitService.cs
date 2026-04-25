namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IUnitService
    {
        Task<IEnumerable<UnitDto>> GetAllUnitsAsync();
        Task<UnitDto?> GetUnitByIdAsync(Guid id);
        Task<IEnumerable<UnitDto>> GetUnitsByPropertyAsync(Guid propertyId);
        Task<IEnumerable<UnitDto>> GetAvailableUnitsAsync();
        Task<UnitDto?> CreateUnitAsync(UnitDto dto);
        Task<bool> UpdateUnitAsync(UnitDto dto);
        Task<bool> DeleteUnitAsync(Guid id);
        Task<bool> UpdateUnitStatusAsync(Guid id, string status);
    }
}
