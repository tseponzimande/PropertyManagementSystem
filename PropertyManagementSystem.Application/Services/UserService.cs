namespace PropertyManagementSystem.Application.Services
{
    public class UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UserService> _logger = logger;

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                return _mapper.Map<IEnumerable<UserDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all users");
                return Enumerable.Empty<UserDto>();
            }
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                return user == null ? null : _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user with ID {UserId}", id);
                return null;
            }
        }

        public async Task<UserDto> CreateUserAsync(UserDto dto, string password, string roleName)
        {
            try
            {
                var existingUser = await _unitOfWork.Users.GetUserByEmailAsync(dto.Email);
                if (existingUser != null)
                    throw new Exception("Email already exists");

                var role = (await _unitOfWork.Roles.GetAllAsync()).FirstOrDefault(r => r.RoleType.Equals(roleName, StringComparison.OrdinalIgnoreCase));

                if (role == null)
                    throw new Exception("Role not found");

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                var user = _mapper.Map<User>(dto);
                user.Id = Guid.NewGuid();
                user.PasswordHash = passwordHash;
                user.RoleId = role.Id;
                user.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Users.CreateAsync(user);
                await _unitOfWork.CompleteAsync();

                var userDto = _mapper.Map<UserDto>(user);
                userDto.Role = role.RoleType;

                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user");
                return null;
            }
        }

        public async Task UpdateUserAsync(UserDto dto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(dto.Id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", dto.Id);
                    return;
                }

                _mapper.Map(dto, user);
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user with ID {UserId}", dto.Id);
            }
        }

        public async Task DeleteUserAsync(Guid id)
        {
            try
            {
                await _unitOfWork.Users.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user with ID {UserId}", id);
            }
        }
    }
}
