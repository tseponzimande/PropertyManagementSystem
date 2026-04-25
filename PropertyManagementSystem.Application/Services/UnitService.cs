namespace PropertyManagementSystem.Application.Services
{
    public class UnitService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UnitService> logger) : IUnitService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UnitService> _logger = logger;

        public async Task<IEnumerable<UnitDto>> GetAllUnitsAsync()
        {
            try
            {
                var units = await _unitOfWork.Units.GetAllAsync();

                return _mapper.Map<IEnumerable<UnitDto>>(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return Enumerable.Empty<UnitDto>();
            }
        }

        public async Task<UnitDto?> GetUnitByIdAsync(Guid id)
        {
            try
            {
                var unit = await _unitOfWork.Units.GetByIdAsync(id);

                return unit == null ? null : _mapper.Map<UnitDto>(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return null;
            }
        }

        public async Task<IEnumerable<UnitDto>> GetUnitsByPropertyAsync(Guid propertyId)
        {
            try
            {
                var units = await _unitOfWork.Units.GetUnitsByPropertyAsync(propertyId);

                return _mapper.Map<IEnumerable<UnitDto>>(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return Enumerable.Empty<UnitDto>();
            }
        }

        public async Task<IEnumerable<UnitDto>> GetAvailableUnitsAsync()
        {
            try
            {
                var units = await _unitOfWork.Units.GetAvailableUnitsAsync();

                return _mapper.Map<IEnumerable<UnitDto>>(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return Enumerable.Empty<UnitDto>();
            }
        }

        public async Task<UnitDto?> CreateUnitAsync(UnitDto dto)
        {
            try
            {
                var unit = _mapper.Map<Unit>(dto);

                unit.Id = Guid.NewGuid();

                await _unitOfWork.Units.CreateAsync(unit);

                await _unitOfWork.CompleteAsync();

                return _mapper.Map<UnitDto>(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return null;
            }
        }

        public async Task<bool> UpdateUnitAsync(UnitDto dto)
        {
            try
            {
                var unit = await _unitOfWork.Units.GetByIdAsync(dto.Id);

                if (unit == null)
                {
                    return false;
                }

                _mapper.Map(dto, unit);

                await _unitOfWork.Units.UpdateAsync(unit);

                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }

        public async Task<bool> DeleteUnitAsync(Guid id)
        {
            try
            {
                var unit = await _unitOfWork.Units.GetByIdAsync(id);

                if (unit == null)
                {
                    return false;
                }

                await _unitOfWork.Units.DeleteAsync(id);

                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }

        public async Task<bool> UpdateUnitStatusAsync(Guid id, string status)
        {
            try
            {
                var unit = await _unitOfWork.Units.GetByIdAsync(id);

                if (unit == null)
                {
                    return false;
                }

                unit.Status = status;

                await _unitOfWork.Units.UpdateAsync(unit);

                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }
    }
}
