namespace PropertyManagementSystem.Infrastructure.Jobs
{
    public class RentReminderJob(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ILogger<RentReminderJob> logger)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INotificationService _notificationService = notificationService;
        private readonly ILogger<RentReminderJob> _logger = logger;

        public async Task Execute()
        {
            try
            {
                _logger.LogInformation("Starting Rent Reminder Job at {Time}", DateTime.UtcNow);

                var leases = await _unitOfWork.Leases.GetAllAsync();
                var activeLeases = leases.Where(l => l.Status == LeaseEnum.Active.ToString());
                var today = DateTime.UtcNow;
                var reminderDate = today.AddDays(5);

                foreach (var lease in activeLeases)
                {
                    var nextPaymentDueDate = new DateTime(today.Year, today.Month, lease.StartDate.Day);
                    if (nextPaymentDueDate < today)
                        nextPaymentDueDate = nextPaymentDueDate.AddMonths(1);

                    if (nextPaymentDueDate.Date == reminderDate.Date)
                    {
                        var payments = await _unitOfWork.Payments.GetPaymentsByLeaseAsync(lease.Id);
                        var currentMonthPayment = payments.FirstOrDefault(p =>
                            p.PaymentDate.Month == today.Month &&
                            p.PaymentDate.Year == today.Year &&
                            p.Status == PaymentEnum.Payment.ToString());

                        if (currentMonthPayment == null)
                        {
                            var notification = await _notificationService.CreateNotificationAsync(new NotificationDto
                            {
                                UserId = lease.TenantId,
                                Type = NotificationEnum.Payment.ToString(),
                                Message = $"Reminder: Your rent of R{lease.RentAmount} is due on {nextPaymentDueDate:MMM dd, yyyy}.",
                                IsRead = false
                            });

                            if (notification != null)
                                _logger.LogInformation("Rent reminder saved for tenant {TenantId}, lease {LeaseId}",
                                    lease.TenantId, lease.Id);
                        }
                    }
                }

                _logger.LogInformation("Rent Reminder Job completed at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing Rent Reminder Job");
            }
        }
    }
}

//namespace PropertyManagementSystem.Infrastructure.HangfireJobs
//{
//    public class RentReminderJob(
//        IUnitOfWork unitOfWork,
//        INotificationService notificationService,
//        IHubContext<NotificationHub> notificationHub,
//        ILogger<RentReminderJob> logger)
//    {
//        private readonly IUnitOfWork _unitOfWork = unitOfWork;
//        private readonly INotificationService _notificationService = notificationService;
//        private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
//        private readonly ILogger<RentReminderJob> _logger = logger;

//        public async Task Execute()
//        {
//            try
//            {
//                _logger.LogInformation("Starting Rent Reminder Job at {Time}", DateTime.UtcNow);

//                var leases = await _unitOfWork.Leases.GetAllAsync();
//                var activeLeases = leases.Where(l => l.Status == LeaseEnum.Active.ToString());

//                var today = DateTime.UtcNow;
//                var reminderDate = today.AddDays(5);

//                foreach (var lease in activeLeases)
//                {

//                    var nextPaymentDueDate = new DateTime(today.Year, today.Month, lease.StartDate.Day);

//                    if (nextPaymentDueDate < today)
//                        nextPaymentDueDate = nextPaymentDueDate.AddMonths(1);

//                    if (nextPaymentDueDate.Date == reminderDate.Date)
//                    {

//                        var payments = await _unitOfWork.Payments.GetPaymentsByLeaseAsync(lease.Id);
//                        var currentMonthPayment = payments.FirstOrDefault(p =>
//                            p.PaymentDate.Month == today.Month &&
//                            p.PaymentDate.Year == today.Year &&
//                            p.Status == PaymentEnum.Payment.ToString());

//                        if (currentMonthPayment == null)
//                        {

//                            var notificationDto = new NotificationDto
//                            {
//                                UserId = lease.TenantId,
//                                Type = NotificationEnum.Payment.ToString(),
//                                Message = $"Reminder: Your rent of ${lease.RentAmount} is due on {nextPaymentDueDate:MMM dd, yyyy}",
//                                IsRead = false
//                            };

//                            var notification = await _notificationService.CreateNotificationAsync(notificationDto);

//                            if (notification != null)
//                            {

//                                await _notificationHub.Clients.User(lease.TenantId.ToString())
//                                    .SendAsync("ReceiveNotification", notification);

//                                _logger.LogInformation("Rent reminder sent to tenant {TenantId} for lease {LeaseId}",
//                                    lease.TenantId, lease.Id);
//                            }
//                        }
//                    }
//                }

//                _logger.LogInformation("Rent Reminder Job completed at {Time}", DateTime.UtcNow);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error executing Rent Reminder Job");
//            }
//        }
//    }
//}