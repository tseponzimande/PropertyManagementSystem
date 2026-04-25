namespace PropertyManagementSystem.Application.Services
{
    public class PropertyService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<PropertyService> logger) : IPropertyService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<PropertyService> _logger = logger;

        #endregion
        #region Constructor

        #endregion

        #region Read Operations

        public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
        {
            try
            {
                var properties = await _unitOfWork.Properties.GetAllAsync();
                return _mapper.Map<IEnumerable<PropertyDto>>(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all properties");
                return Enumerable.Empty<PropertyDto>();
            }
        }

        public async Task<PropertyDto?> GetPropertyByIdAsync(Guid id)
        {
            try
            {
                var property = await _unitOfWork.Properties.GetByIdAsync(id);
                return property == null ? null : _mapper.Map<PropertyDto>(property);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get property with ID {PropertyId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerAsync(Guid ownerId)
        {
            try
            {
                var properties = await _unitOfWork.Properties.GetPropertiesByOwnerAsync(ownerId);
                return _mapper.Map<IEnumerable<PropertyDto>>(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get properties for owner {OwnerId}", ownerId);
                return Enumerable.Empty<PropertyDto>();
            }
        }

        public async Task<IEnumerable<PropertyDto>> GetApprovedPropertiesAsync()
        {
            try
            {
                var properties = await _unitOfWork.Properties.GetApprovedPropertiesAsync();
                return _mapper.Map<IEnumerable<PropertyDto>>(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get approved properties");
                return Enumerable.Empty<PropertyDto>();
            }
        }

        #endregion

        #region Create / Update / Delete

        public async Task<PropertyDto?> CreatePropertyAsync(PropertyDto dto)
        {
            try
            {
                var owner = await _unitOfWork.Users.GetByIdAsync(dto.OwnerId);
                if (owner == null)
                {
                    _logger.LogWarning("Owner with ID {OwnerId} not found", dto.OwnerId);
                    return null;
                }

                var property = _mapper.Map<Property>(dto);
                property.Id = Guid.NewGuid();
                property.Status = StatusEnum.Pending.ToString();
                property.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Properties.CreateAsync(property);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<PropertyDto>(property);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create property");
                return null;
            }
        }

        public async Task<bool> UpdatePropertyAsync(PropertyDto dto)
        {
            try
            {
                var property = await _unitOfWork.Properties.GetByIdAsync(dto.Id);
                if (property == null)
                {
                    _logger.LogWarning("Property with ID {PropertyId} not found", dto.Id);
                    return false;
                }

                _mapper.Map(dto, property);
                await _unitOfWork.Properties.UpdateAsync(property);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update property with ID {PropertyId}", dto.Id);
                return false;
            }
        }

        public async Task<bool> DeletePropertyAsync(Guid id)
        {
            try
            {
                var property = await _unitOfWork.Properties.GetByIdAsync(id);
                if (property == null)
                {
                    _logger.LogWarning("Property with ID {PropertyId} not found", id);
                    return false;
                }

                await _unitOfWork.Properties.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete property with ID {PropertyId}", id);
                return false;
            }
        }

        #endregion

        #region Status Management

        public async Task<bool> ApprovePropertyAsync(Guid id)
        {
            try
            {
                var property = await _unitOfWork.Properties.GetByIdAsync(id);
                if (property == null)
                {
                    _logger.LogWarning("Property with ID {PropertyId} not found", id);
                    return false;
                }

                property.Status = StatusEnum.Approved.ToString();
                await _unitOfWork.Properties.UpdateAsync(property);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Property {PropertyId} approved", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to approve property with ID {PropertyId}", id);
                return false;
            }
        }

        public async Task<bool> RejectPropertyAsync(Guid id)
        {
            try
            {
                var property = await _unitOfWork.Properties.GetByIdAsync(id);
                if (property == null)
                {
                    _logger.LogWarning("Property with ID {PropertyId} not found", id);
                    return false;
                }

                property.Status = StatusEnum.Reject.ToString();
                await _unitOfWork.Properties.UpdateAsync(property);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Property {PropertyId} rejected", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reject property with ID {PropertyId}", id);
                return false;
            }
        }

        #endregion
    }
}
