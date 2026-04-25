namespace PropertyManagementSystem.Application.IRepository
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetNotificationsByUserAsync(Guid userId);
    }
}
