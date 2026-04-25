namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();
        Task<PropertyDto?> GetPropertyByIdAsync(Guid id);
        Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerAsync(Guid ownerId);
        Task<IEnumerable<PropertyDto>> GetApprovedPropertiesAsync();
        Task<PropertyDto?> CreatePropertyAsync(PropertyDto dto);
        Task<bool> UpdatePropertyAsync(PropertyDto dto);
        Task<bool> DeletePropertyAsync(Guid id);
        Task<bool> ApprovePropertyAsync(Guid id);
        Task<bool> RejectPropertyAsync(Guid id);
    }
}