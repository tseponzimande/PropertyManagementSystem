namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IPropertyRepository Properties { get; }
        IUnitRepository Units { get; }
        ILeaseRepository Leases { get; }
        IPaymentRepository Payments { get; }
        IMaintenanceRequestRepository MaintenanceRequests { get; }
        IMessageRepository Messages { get; }
        INotificationRepository Notifications { get; }
        IAuditLogRepository AuditLog { get; }

        Task<int> CompleteAsync();
    }
}
