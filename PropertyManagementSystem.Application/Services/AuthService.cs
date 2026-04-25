namespace PropertyManagementSystem.Application.Services
{
    public class AuthService(IUnitOfWork unitOfWork,IEmailService emailService,IJwtService jwtService,ILogger<AuthService> logger) : IAuthService
    {
        #region Dependencies

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IEmailService _emailService = emailService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly ILogger<AuthService> _logger = logger;

        #endregion

        #region Constructor

        #endregion

        #region Methods

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request,string ipAddress,string? userAgent)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);

                if (user == null || !user.IsActive)
                    return null;

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return null;

                user.LastLoginDate = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();

                var roles = new List<string> { user.Role.RoleType };
                var accessToken = _jwtService.GenerateToken(user, roles);

                return new LoginResponseDto
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.RoleType,
                    AccessToken = accessToken,
                    RefreshToken = Guid.NewGuid().ToString(),
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(60),
                    RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return null;
            }
        }

        public async Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto request,string ipAddress,string? userAgent)
        {
            try
            {
                if (request.Password != request.ConfirmPassword)
                    throw new Exception("Passwords do not match");

                var existingUser = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
                if (existingUser != null)
                    throw new Exception("Email already exists");

                var role = (await _unitOfWork.Roles.GetAllAsync())
                    .FirstOrDefault(r => r.RoleType.Equals(request.Role, StringComparison.OrdinalIgnoreCase));

                if (role == null)
                    throw new Exception("Invalid role");

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    RoleId = role.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.CreateAsync(user);
                await _unitOfWork.CompleteAsync();

                await _emailService.SendWelcomeEmailAsync(user.Email, user.Name);

                var token = _jwtService.GenerateToken(user, new List<string> { role.RoleType });

                return new LoginResponseDto
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = role.RoleType,
                    AccessToken = token,
                    RefreshToken = Guid.NewGuid().ToString(),
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(60),
                    RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return null;
            }
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            try
            {
                await Task.CompletedTask;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return false;

                if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                    return false;

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Change password failed");
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
                if (user == null)
                    return true;

                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

                user.PasswordResetToken = token;
                user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();

                var body = $"Your password reset token is: {token}";
                await _emailService.SendEmailAsync(user.Email, "Password Reset", body);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forgot password failed");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
                if (user == null)
                    return false;

                if (user.PasswordResetToken != request.Token || user.PasswordResetTokenExpiry < DateTime.UtcNow)
                    return false;

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiry = null;

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reset password failed");
                return false;
            }
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
                return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _unitOfWork.Users.GetUserByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return null;
            }
        }

        #endregion
    }
}