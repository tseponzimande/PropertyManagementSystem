namespace PropertyManagementSystem.Application.Services
{
    public class NotificationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<NotificationService> logger) : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<NotificationService> _logger = logger;

        public async Task<IEnumerable<NotificationDto>> GetNotificationsByUserAsync(Guid userId)
        {
            try
            {
                var notifications = await _unitOfWork.Notifications.GetNotificationsByUserAsync(userId);
                return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return Enumerable.Empty<NotificationDto>();
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsByUserAsync(Guid userGuid)
        {
            try
            {
                var res = await _unitOfWork.Notifications.GetNotificationsByUserAsync(userGuid);
                return _mapper.Map<IEnumerable<NotificationDto>>(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return Enumerable.Empty<NotificationDto>();
            }
        }

        public async Task<NotificationDto?> CreateNotificationAsync(NotificationDto dto)
        {
            var notification = _mapper.Map<Notification>(dto);
            notification.Id = Guid.NewGuid();
            notification.IsRead = false;

            await _unitOfWork.Notifications.CreateAsync(notification);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<bool> MarkAsReadAsync(Guid id)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (notification == null) return false;

            notification.IsRead = true;
            await _unitOfWork.Notifications.UpdateAsync(notification);
            await _unitOfWork.CompleteAsync();

            return true;
        }

    }
}