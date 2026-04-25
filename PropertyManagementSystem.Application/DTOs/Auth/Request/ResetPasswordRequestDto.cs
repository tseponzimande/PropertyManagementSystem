namespace PropertyManagementSystem.Application.DTOs.Auth.Request
{
    public class ResetPasswordRequestDto
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
