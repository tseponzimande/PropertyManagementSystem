namespace PropertyManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    //[AllowAnonymous]
    [Authorize]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        #region Dependencies

        private readonly IAuthService _authService = authService;

        #endregion

        #region Methods

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                Request.Headers["User-Agent"].ToString());

            if (result == null)
                return Unauthorized("Invalid credentials");

            return Ok(result);
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request,HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",Request.Headers["User-Agent"].ToString());

            if (result == null)
                return BadRequest("Registration failed");

            return Ok(result);
        }

        /// <summary>
        /// ForgotPassword
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            await _authService.ForgotPasswordAsync(request);
            return Ok("If the email exists, a reset link has been sent.");
        }



        /// <summary>
        /// ResetPassword
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            var success = await _authService.ResetPasswordAsync(request);
            if (!success)
                return BadRequest("Invalid token or expired");

            return Ok("Password reset successful");
        }


        /// <summary>
        /// ChangePassword
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _authService.ChangePasswordAsync(userId, request);

            if (!success)
                return BadRequest("Password change failed");

            return Ok("Password updated successfully");
        }


        /// <summary>
        /// Logout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _authService.LogoutAsync(userId);
            return Ok("Logged out");
        }

        #endregion
    }
}
