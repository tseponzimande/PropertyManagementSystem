namespace PropertyManagementSystem.Infrastructure.Jobs
{
    public class MaintenanceFollowUpJob(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ILogger<MaintenanceFollowUpJob> logger)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INotificationService _notificationService = notificationService;
        private readonly ILogger<MaintenanceFollowUpJob> _logger = logger;

        public async Task Execute()
        {
            try
            {
                _logger.LogInformation("Starting Maintenance Follow-Up Job at {Time}", DateTime.UtcNow);

                var requests = await _unitOfWork.MaintenanceRequests.GetAllAsync();
                var today = DateTime.UtcNow;

                foreach (var request in requests)
                {
                    var daysSinceCreated = (today - request.CreatedAt).Days;

                    if (request.Status == MaintenanceRequestEnum.InProgress.ToString()
                        && daysSinceCreated >= 7
                        && daysSinceCreated % 7 == 0)
                    {
                        var tenantNotification = new NotificationDto
                        {
                            UserId = request.TenantId,
                            Type = NotificationEnum.Maintenance.ToString(),
                            Message = $"Your maintenance request '{request.Title}' has been in progress for {daysSinceCreated} days.",
                            IsRead = false
                        };

                        await _notificationService.CreateNotificationAsync(tenantNotification);

                        var unit = await _unitOfWork.Units.GetByIdAsync(request.UnitId);
                        if (unit != null)
                        {
                            var property = await _unitOfWork.Properties.GetByIdAsync(unit.PropertyId);
                            if (property != null)
                            {
                                await _notificationService.CreateNotificationAsync(new NotificationDto
                                {
                                    UserId = property.OwnerId,
                                    Type = NotificationEnum.Maintenance.ToString(),
                                    Message = $"Reminder: Maintenance request '{request.Title}' has been in progress for {daysSinceCreated} days.",
                                    IsRead = false
                                });
                            }
                        }

                        _logger.LogInformation("In-progress follow-up saved for request {RequestId}", request.Id);
                    }
                }

                _logger.LogInformation("Maintenance Follow-Up Job completed at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing Maintenance Follow-Up Job");
            }
        }
    }
}

//namespace PropertyManagementSystem.Infrastructure.HangfireJobs
//{
//    public class MaintenanceFollowUpJob(
//        IUnitOfWork unitOfWork,
//        INotificationService notificationService,
//        IHubContext<NotificationHub> notificationHub,
//        ILogger<MaintenanceFollowUpJob> logger)
//    {
//        private readonly IUnitOfWork _unitOfWork = unitOfWork;
//        private readonly INotificationService _notificationService = notificationService;
//        private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
//        private readonly ILogger<MaintenanceFollowUpJob> _logger = logger;

//        public async Task Execute()
//        {
//            try
//            {
//                _logger.LogInformation("Starting Maintenance Follow-Up Job at {Time}", DateTime.UtcNow);

//                var requests = await _unitOfWork.MaintenanceRequests.GetAllAsync();
//                var today = DateTime.UtcNow;

//                foreach (var request in requests)
//                {
//                    var daysSinceCreated = (today - request.CreatedAt).Days;

//                    if (request.Status == MaintenanceRequestEnum.Open.ToString() && daysSinceCreated >= 3)
//                    {

//                        var unit = await _unitOfWork.Units.GetByIdAsync(request.UnitId);
//                        if (unit != null)
//                        {
//                            var property = await _unitOfWork.Properties.GetByIdAsync(unit.PropertyId);
//                            if (property != null)
//                            {
//                                var ownerNotification = new NotificationDto
//                                {
//                                    UserId = property.OwnerId,
//                                    Type = NotificationEnum.Maintenance.ToString(),
//                                    Message = $"Reminder: Maintenance request '{request.Title}' for Unit {unit.UnitNumber} has been open for {daysSinceCreated} days.",
//                                    IsRead = false
//                                };

//                                var notification = await _notificationService.CreateNotificationAsync(ownerNotification);
//                                if (notification != null)
//                                {
//                                    await _notificationHub.Clients.User(property.OwnerId.ToString())
//                                        .SendAsync("ReceiveNotification", notification);
//                                }
//                                _logger.LogInformation("Follow-up notification sent for maintenance request {RequestId}", request.Id);
//                            }
//                        }
//                    }

//                    if (request.Status == MaintenanceRequestEnum.InProgress.ToString() && daysSinceCreated >= 7)
//                    {
//                        var tenantNotification = new NotificationDto
//                        {
//                            UserId = request.TenantId,
//                            Type = NotificationEnum.Maintenance.ToString(),
//                            Message = $"Update: Your maintenance request '{request.Title}' is still in progress. We appreciate your patience.",
//                            IsRead = false
//                        };

//                        var tenantNotif = await _notificationService.CreateNotificationAsync(tenantNotification);
//                        if (tenantNotif != null)
//                        {
//                            await _notificationHub.Clients.User(request.TenantId.ToString())
//                                .SendAsync("ReceiveNotification", tenantNotif);
//                        }

//                        var unit = await _unitOfWork.Units.GetByIdAsync(request.UnitId);
//                        if (unit != null)
//                        {
//                            var property = await _unitOfWork.Properties.GetByIdAsync(unit.PropertyId);
//                            if (property != null)
//                            {
//                                var ownerNotification = new NotificationDto
//                                {
//                                    UserId = property.OwnerId,
//                                    Type = NotificationEnum.Maintenance.ToString(),
//                                    Message = $"Reminder: Maintenance request '{request.Title}' has been in progress for {daysSinceCreated} days.",
//                                    IsRead = false
//                                };

//                                var ownerNotif = await _notificationService.CreateNotificationAsync(ownerNotification);
//                                if (ownerNotif != null)
//                                {
//                                    await _notificationHub.Clients.User(property.OwnerId.ToString())
//                                        .SendAsync("ReceiveNotification", ownerNotif);
//                                }
//                            }
//                        }

//                        _logger.LogInformation("In-progress follow-up sent for maintenance request {RequestId}", request.Id);
//                    }
//                }
//                _logger.LogInformation("Maintenance Follow-Up Job completed at {Time}", DateTime.UtcNow);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error executing Maintenance Follow-Up Job");
//            }
//        }
//    }
//}