namespace PropertyManagementSystem.UI.Components.Pages.Auth
{
    public partial class ForgotPassword
    {
        #region Dependencies

        [Inject]
        private NavigationManager Nav { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        #endregion

        #region

        private readonly ForgotPasswordRequestDto model = new();

        private bool isLoading = false;

        private bool submitted = false;

        private string sentEmail = string.Empty;

        #endregion

        private async Task HandleSubmit(ForgotPasswordRequestDto m)
        {
            isLoading = true;

            try
            {
                sentEmail = m.Email;
                await AuthService.ForgotPasswordAsync(m);
                submitted = true;
            }
            catch
            {
                submitted = true;
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
