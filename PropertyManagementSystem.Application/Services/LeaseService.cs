namespace PropertyManagementSystem.Application.Services
{
    public class LeaseService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<LeaseService> logger) : ILeaseService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<LeaseService> _logger = logger;

        #endregion

        #region Get(s)

        public async Task<IEnumerable<LeaseDto>> GetAllLeasesAsync()
        {
            try
            {
                var leases = await _unitOfWork.Leases.GetAllAsync();
                return _mapper.Map<IEnumerable<LeaseDto>>(leases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all leases");
                return Enumerable.Empty<LeaseDto>();
            }
        }

        public async Task<LeaseDto?> GetLeaseByIdAsync(Guid id)
        {
            try
            {
                var lease = await _unitOfWork.Leases.GetByIdAsync(id);
                return lease == null ? null : _mapper.Map<LeaseDto>(lease);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get lease {LeaseId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<LeaseDto>> GetLeasesByUnitAsync(Guid unitId)
        {
            try
            {
                var leases = await _unitOfWork.Leases.GetLeasesByUnitAsync(unitId);
                return _mapper.Map<IEnumerable<LeaseDto>>(leases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return Enumerable.Empty<LeaseDto>();
            }
        }

        public async Task<IEnumerable<LeaseDto>> GetLeasesByTenantAsync(Guid tenantId)
        {
            try
            {
                var leases = await _unitOfWork.Leases.GetLeasesByTenantAsync(tenantId);
                return _mapper.Map<IEnumerable<LeaseDto>>(leases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return Enumerable.Empty<LeaseDto>();
            }
        }

        #endregion

        #region C / U/ D

        public async Task<LeaseDto?> CreateLeaseAsync(LeaseDto dto)
        {
            try
            {
                var lease = _mapper.Map<Lease>(dto);
                lease.Id = Guid.NewGuid();
                lease.Status = LeaseEnum.Active.ToString();
                //lease.Status = "Active";

                await _unitOfWork.Leases.CreateAsync(lease);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<LeaseDto>(lease);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create lease");
                return null;
            }
        }

        public async Task<bool> UpdateLeaseAsync(LeaseDto dto)
        {
            try
            {
                var lease = await _unitOfWork.Leases.GetByIdAsync(dto.Id);
                if (lease == null) return false;

                _mapper.Map(dto, lease);
                await _unitOfWork.Leases.UpdateAsync(lease);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }

        public async Task<bool> DeleteLeaseAsync(Guid id)
        {
            try
            {
                await _unitOfWork.Leases.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }

        #endregion

        #region Status Management

        public async Task<bool> TerminateLeaseAsync(Guid id)
        {
            try
            {
                var lease = await _unitOfWork.Leases.GetByIdAsync(id);
                if (lease == null) return false;

                //lease.Status = "Terminated";
                lease.Status = LeaseEnum.Terminated.ToString();
                await _unitOfWork.Leases.UpdateAsync(lease);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }

        #endregion
    }
}