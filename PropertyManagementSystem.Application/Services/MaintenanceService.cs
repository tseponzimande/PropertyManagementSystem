namespace PropertyManagementSystem.Application.Services
{
    public class MaintenanceService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<MaintenanceService> logger) : IMaintenanceService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<MaintenanceService> _logger = logger;

        #endregion

        public async Task<IEnumerable<MaintenanceRequestDto>> GetRequestsByUnitAsync(Guid unitId)
        {
            var requests = await _unitOfWork.MaintenanceRequests.GetMaintenanceRequestsByUnitAsync(unitId);
            return _mapper.Map<IEnumerable<MaintenanceRequestDto>>(requests);
        }

        public async Task<MaintenanceRequestDto?> CreateRequestAsync(MaintenanceRequestDto dto)
        {
            var request = _mapper.Map<MaintenanceRequest>(dto);
            request.Id = Guid.NewGuid();
            //request.Status = "Open";
            request.Status = MaintenanceRequestEnum.Open.ToString();

            await _unitOfWork.MaintenanceRequests.CreateAsync(request);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<MaintenanceRequestDto>(request);
        }

        public async Task<bool> UpdateRequestStatusAsync(Guid id, string status)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetByIdAsync(id);
            if (request == null) return false;

            request.Status = status;
            await _unitOfWork.MaintenanceRequests.UpdateAsync(request);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}