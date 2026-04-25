namespace PropertyManagementSystem.Application.DTOs.Auth.Request
{
    public class LoginRequestDto
    {
        #region Properties

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        #endregion
    }
}
