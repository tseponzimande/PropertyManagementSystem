namespace PropertyManagementSystem.Infrastructure.Jobs
{
    public class LeaseExpiryJob(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IEmailService emailService,
        ILogger<LeaseExpiryJob> logger)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IEmailService _emailService = emailService;
        private readonly ILogger<LeaseExpiryJob> _logger = logger;

        public async Task Execute()
        {
            try
            {
                _logger.LogInformation("Starting Lease Expiry Job at {Time}", DateTime.UtcNow);

                var leases = await _unitOfWork.Leases.GetAllAsync();
                var activeLeases = leases.Where(l => l.Status == LeaseEnum.Active.ToString());
                var today = DateTime.UtcNow;

                foreach (var lease in activeLeases)
                {
                    var daysUntilExpiry = (lease.EndDate.Date - today.Date).Days;

                    if (daysUntilExpiry == 30 || daysUntilExpiry == 14 || daysUntilExpiry == 7)
                        await SendExpiryNotifications(lease, daysUntilExpiry);
                    else if (daysUntilExpiry < 0 && lease.Status == LeaseEnum.Active.ToString())
                        await AutoTerminateLease(lease);
                }

                _logger.LogInformation("Lease Expiry Job completed at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing Lease Expiry Job");
            }
        }

        private async Task SendExpiryNotifications(Lease lease, int daysUntilExpiry)
        {
            try
            {
                var tenant = await _unitOfWork.Users.GetByIdAsync(lease.TenantId);
                if (tenant == null) return;

                var unit = await _unitOfWork.Units.GetByIdAsync(lease.UnitId);
                if (unit == null) return;

                var property = await _unitOfWork.Properties.GetByIdAsync(unit.PropertyId);
                if (property == null) return;

                var owner = await _unitOfWork.Users.GetByIdAsync(property.OwnerId);
                if (owner == null) return;

                var message = daysUntilExpiry switch
                {
                    30 => $"Your lease for Unit {unit.UnitNumber} at {property.Address} will expire in 30 days ({lease.EndDate:MMM dd, yyyy}).",
                    14 => $"Reminder: Your lease for Unit {unit.UnitNumber} at {property.Address} will expire in 2 weeks ({lease.EndDate:MMM dd, yyyy}).",
                    7 => $"URGENT: Your lease for Unit {unit.UnitNumber} at {property.Address} expires in 7 days ({lease.EndDate:MMM dd, yyyy}).",
                    _ => $"Your lease will expire in {daysUntilExpiry} days ({lease.EndDate:MMM dd, yyyy})."
                };

                // Save notification to DB (real-time push will come back with SignalR)
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    UserId = lease.TenantId,
                    Type = NotificationEnum.Lease.ToString(),
                    Message = message,
                    IsRead = false
                });

                await _emailService.SendLeaseExpiryReminderAsync(tenant.Email, tenant.Name, lease.EndDate);

                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    UserId = property.OwnerId,
                    Type = NotificationEnum.Lease.ToString(),
                    Message = $"Lease expiry reminder: Tenant {tenant.Name} in Unit {unit.UnitNumber} expires in {daysUntilExpiry} days.",
                    IsRead = false
                });

                _logger.LogInformation("Lease expiry notifications saved for lease {LeaseId}", lease.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending expiry notifications for lease {LeaseId}", lease.Id);
            }
        }

        private async Task AutoTerminateLease(Lease lease)
        {
            try
            {
                lease.Status = LeaseEnum.Expired.ToString();
                await _unitOfWork.Leases.UpdateAsync(lease);
                await _unitOfWork.CompleteAsync();

                var unit = await _unitOfWork.Units.GetByIdAsync(lease.UnitId);
                if (unit != null)
                {
                    unit.Status = UnitEnum.Available.ToString();
                    await _unitOfWork.Units.UpdateAsync(unit);
                    await _unitOfWork.CompleteAsync();
                }

                var tenant = await _unitOfWork.Users.GetByIdAsync(lease.TenantId);
                var property = unit != null ? await _unitOfWork.Properties.GetByIdAsync(unit.PropertyId) : null;

                if (tenant != null && unit != null && property != null)
                {
                    await _notificationService.CreateNotificationAsync(new NotificationDto
                    {
                        UserId = lease.TenantId,
                        Type = NotificationEnum.Lease.ToString(),
                        Message = $"Your lease for Unit {unit.UnitNumber} at {property.Address} has expired and been automatically terminated.",
                        IsRead = false
                    });

                    await _notificationService.CreateNotificationAsync(new NotificationDto
                    {
                        UserId = property.OwnerId,
                        Type = NotificationEnum.Lease.ToString(),
                        Message = $"Lease for tenant {tenant.Name} in Unit {unit.UnitNumber} at {property.Address} has expired. The unit is now available.",
                        IsRead = false
                    });
                }

                _logger.LogInformation("Lease {LeaseId} auto-terminated", lease.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error auto-terminating lease {LeaseId}", lease.Id);
            }
        }
    }
}


//namespace PropertyManagementSystem.Infrastructure.HangfireJobs
//{
//    public class LeaseExpiryJob(
//        IUnitOfWork unitOfWork,
//        INotificationService notificationService,
//        IEmailService emailService,
//        IHubContext<NotificationHub> notificationHub,
//        ILogger<LeaseExpiryJob> logger)
//    {
//        private readonly IUnitOfWork _unitOfWork = unitOfWork;
//        private readonly INotificationService _notificationService = notificationService;
//        private readonly IEmailService _emailService = emailService;
//        private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
//        private readonly ILogger<LeaseExpiryJob> _logger = logger;

//        public async Task Execute()
//        {
//            try
//            {
//                _logger.LogInformation("Starting Lease Expiry Job at {Time}", DateTime.UtcNow);

//                var leases = await _unitOfWork.Leases.GetAllAsync();
//                var activeLeases = leases.Where(l => l.Status == LeaseEnum.Active.ToString());

//                var today = DateTime.UtcNow;

//                foreach (var lease in activeLeases)
//                {
//                    var daysUntilExpiry = (lease.EndDate.Date - today.Date).Days;

//                    if (daysUntilExpiry == 30 || daysUntilExpiry == 14 || daysUntilExpiry == 7)
//                    {
//                        await SendExpiryNotifications(lease, daysUntilExpiry);
//                    }

//                    else if (daysUntilExpiry < 0 && lease.Status == LeaseEnum.Active.ToString())
//                    {
//                        await AutoTerminateLease(lease);
//                    }
//                }

//                _logger.LogInformation("Lease Expiry Job completed at {Time}", DateTime.UtcNow);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error executing Lease Expiry Job");
//            }
//        }

//        private async Task SendExpiryNotifications(Lease lease, int daysUntilExpiry)
//        {
//            try
//            {

//                var tenant = await _unitOfWork.Users.GetByIdAsync(lease.TenantId);
//                if (tenant == null)
//                {
//                    _logger.LogWarning("Tenant {TenantId} not found for lease {LeaseId}", lease.TenantId, lease.Id);
//                    return;
//                }

//                var unit = await _unitOfWork.Units.GetByIdAsync(lease.UnitId);
//                if (unit == null)
//                {
//                    _logger.LogWarning("Unit {UnitId} not found for lease {LeaseId}", lease.UnitId, lease.Id);
//                    return;
//                }

//                var property = await _unitOfWork.Properties.GetByIdAsync(unit.PropertyId);
//                if (property == null)
//                {
//                    _logger.LogWarning("Property {PropertyId} not found for unit {UnitId}", unit.PropertyId, unit.Id);
//                    return;
//                }

//                var owner = await _unitOfWork.Users.GetByIdAsync(property.OwnerId);
//                if (owner == null)
//                {
//                    _logger.LogWarning("Owner {OwnerId} not found for property {PropertyId}", property.OwnerId, property.Id);
//                    return;
//                }

//                var message = daysUntilExpiry switch
//                {
//                    30 => $"Your lease for Unit {unit.UnitNumber} at {property.Address} will expire in 30 days ({lease.EndDate:MMM dd, yyyy}). Please contact your landlord to discuss renewal.",
//                    14 => $"Reminder: Your lease for Unit {unit.UnitNumber} at {property.Address} will expire in 2 weeks ({lease.EndDate:MMM dd, yyyy}).",
//                    7 => $"URGENT: Your lease for Unit {unit.UnitNumber} at {property.Address} will expire in 7 days ({lease.EndDate:MMM dd, yyyy}). Please take action immediately.",
//                    _ => $"Your lease will expire in {daysUntilExpiry} days ({lease.EndDate:MMM dd, yyyy})."
//                };


//                var tenantNotification = new NotificationDto
//                {
//                    UserId = lease.TenantId,
//                    Type = NotificationEnum.Lease.ToString(),
//                    Message = message,
//                    IsRead = false
//                };

//                var tenantNotif = await _notificationService.CreateNotificationAsync(tenantNotification);
//                if (tenantNotif != null)
//                {
//                    await _notificationHub.Clients.User(lease.TenantId.ToString())
//                        .SendAsync("ReceiveNotification", tenantNotif);
//                }

//                await _emailService.SendLeaseExpiryReminderAsync(tenant.Email, tenant.Name, lease.EndDate);

//                var ownerMessage = $"Lease expiry reminder: Tenant {tenant.Name} in Unit {unit.UnitNumber} at {property.Address} has a lease expiring in {daysUntilExpiry} days ({lease.EndDate:MMM dd, yyyy}).";
//                var ownerNotification = new NotificationDto
//                {
//                    UserId = property.OwnerId,
//                    Type = NotificationEnum.Lease.ToString(),
//                    Message = ownerMessage,
//                    IsRead = false
//                };

//                var ownerNotif = await _notificationService.CreateNotificationAsync(ownerNotification);
//                if (ownerNotif != null)
//                {
//                    await _notificationHub.Clients.User(property.OwnerId.ToString())
//                        .SendAsync("ReceiveNotification", ownerNotif);
//                }

//                _logger.LogInformation("Lease expiry notifications sent for lease {LeaseId} ({DaysUntilExpiry} days)",
//                    lease.Id, daysUntilExpiry);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error sending expiry notifications for lease {LeaseId}", lease.Id);
//            }
//        }

//        private async Task AutoTerminateLease(Lease lease)
//        {
//            try
//            {
//                lease.Status = LeaseEnum.Expired.ToString();
//                await _unitOfWork.Leases.UpdateAsync(lease);
//                await _unitOfWork.CompleteAsync();

//                var unit = await _unitOfWork.Units.GetByIdAsync(lease.UnitId);
//                if (unit != null)
//                {
//                    unit.Status = UnitEnum.Available.ToString();
//                    await _unitOfWork.Units.UpdateAsync(unit);
//                    await _unitOfWork.CompleteAsync();
//                }

//                var tenant = await _unitOfWork.Users.GetByIdAsync(lease.TenantId);
//                var property = unit != null ? await _unitOfWork.Properties.GetByIdAsync(unit.PropertyId) : null;

//                if (tenant != null && unit != null && property != null)
//                {
//                    var tenantMessage = $"Your lease for Unit {unit.UnitNumber} at {property.Address} has expired and been automatically terminated.";
//                    var tenantNotification = new NotificationDto
//                    {
//                        UserId = lease.TenantId,
//                        Type = NotificationEnum.Lease.ToString(),
//                        Message = tenantMessage,
//                        IsRead = false
//                    };

//                    var tenantNotif = await _notificationService.CreateNotificationAsync(tenantNotification);
//                    if (tenantNotif != null)
//                    {
//                        await _notificationHub.Clients.User(lease.TenantId.ToString())
//                            .SendAsync("ReceiveNotification", tenantNotif);
//                    }

//                    var ownerMessage = $"The lease for tenant {tenant.Name} in Unit {unit.UnitNumber} at {property.Address} has expired and been automatically terminated. The unit is now available.";
//                    var ownerNotification = new NotificationDto
//                    {
//                        UserId = property.OwnerId,
//                        Type = NotificationEnum.Lease.ToString(),
//                        Message = ownerMessage,
//                        IsRead = false
//                    };

//                    var ownerNotif = await _notificationService.CreateNotificationAsync(ownerNotification);
//                    if (ownerNotif != null)
//                    {
//                        await _notificationHub.Clients.User(property.OwnerId.ToString())
//                            .SendAsync("ReceiveNotification", ownerNotif);
//                    }
//                }

//                _logger.LogInformation("Lease {LeaseId} auto-terminated due to expiry", lease.Id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error auto-terminating lease {LeaseId}", lease.Id);
//            }
//        }
//    }
//}