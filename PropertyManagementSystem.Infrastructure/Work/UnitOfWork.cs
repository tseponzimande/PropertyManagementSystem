namespace PropertyManagementSystem.Infrastructure.Work
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PropertyManagementDbContext _context;

        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public IPropertyRepository Properties { get; }
        public IUnitRepository Units { get; }
        public ILeaseRepository Leases { get; }
        public IPaymentRepository Payments { get; }
        public IMaintenanceRequestRepository MaintenanceRequests { get; }
        public IMessageRepository Messages { get; }
        public INotificationRepository Notifications { get; }
        public IAuditLogRepository AuditLog { get; }

        public UnitOfWork(
            PropertyManagementDbContext context,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPropertyRepository propertyRepository,
            IUnitRepository unitRepository,
            ILeaseRepository leaseRepository,
            IPaymentRepository paymentRepository,
            IMaintenanceRequestRepository maintenanceRequestRepository,
            IMessageRepository messageRepository,
            INotificationRepository notificationRepository,
            IAuditLogRepository auditLogRepository
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            Roles = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            Properties = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
            Units = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
            Leases = leaseRepository ?? throw new ArgumentNullException(nameof(leaseRepository));
            Payments = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            MaintenanceRequests = maintenanceRequestRepository ?? throw new ArgumentNullException(nameof(maintenanceRequestRepository));
            Messages = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            Notifications = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            AuditLog = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
