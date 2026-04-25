namespace PropertyManagementSystem.Application.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetNotificationsByUserAsync(Guid userId);
        Task<NotificationDto?> CreateNotificationAsync(NotificationDto dto);
        Task<bool> MarkAsReadAsync(Guid id);
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsByUserAsync(Guid userGuid);
    }
}
