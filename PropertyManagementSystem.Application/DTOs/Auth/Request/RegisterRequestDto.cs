namespace PropertyManagementSystem.Application.DTOs.Auth.Request
{
    public class RegisterRequestDto
    {
        #region Properties

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string ConfirmPassword { get; set; } = null!;

        public string Role { get; set; } = null!;

        #endregion
    }
}
