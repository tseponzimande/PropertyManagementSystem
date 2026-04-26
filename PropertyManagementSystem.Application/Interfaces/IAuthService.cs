namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string ipAddress, string? userAgent);
        Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto request, string ipAddress, string? userAgent);
        Task<bool> LogoutAsync(Guid userId);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<bool> ValidateUserAsync(string email, string password);
        Task<User?> GetUserByEmailAsync(string email);
    }
}