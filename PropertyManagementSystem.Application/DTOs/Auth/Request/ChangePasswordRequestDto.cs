namespace PropertyManagementSystem.Application.DTOs.Auth.Request
{
    public class ChangePasswordRequestDto
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
