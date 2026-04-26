namespace PropertyManagementSystem.UI.Components.Pages.Auth
{
    public partial class ResetPassword
    {
        #region Dependencies

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NavigationManager Nav { get; set; } = null!;

        #endregion

        #region Parameter

        [SupplyParameterFromQuery]
        private string? Token { get; set; }


        [SupplyParameterFromQuery]
        private string? Email { get; set; }

        #endregion

        #region Fields

        private readonly ResetPasswordRequestDto model = new();

        private string confirmPassword = string.Empty;

        private bool isLoading = false;

        private bool resetSuccess = false;

        private string? errorMessage = null;

        #endregion


        protected override void OnParametersSet()
        {
            model.Token = Token ?? string.Empty;
            model.Email = Email ?? string.Empty;
        }

        private async Task Reset(ResetPasswordRequestDto submitted)
        {
            isLoading = true;
            errorMessage = null;

            try
            {
                var success = await AuthService.ResetPasswordAsync(submitted);

                if (success)
                {
                    resetSuccess = true;
                }
                else
                {
                    errorMessage = "The reset link is invalid or has expired. Please request a new one.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = "An unexpected error occurred. Please try again.";
                Console.Error.WriteLine(ex);
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}

