namespace PropertyManagementSystem.UI.Components.Pages.Auth
{
    public partial class Login
    {

        [Inject]
        private IAuthService _AuthService { get; set; } = null!;

        [Inject]
        private AuthStateService _AuthStateService { get; set; } = null!;

        [Inject]
        private NavigationManager _NavigationManager { get; set; } = null!;

        private readonly LoginRequestDto model = new();
        private bool isLoading = false;
        private string? errorMessage = null;


        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }


        private async Task UserLogin(LoginRequestDto submitted)
        {
            isLoading = true;
            errorMessage = null;

            try
            {
                var result = await _AuthService.LoginAsync(submitted, string.Empty, null);

                if (result is null)
                {
                    errorMessage = "Invalid email or password. Please try again.";
                    return;
                }


                await _AuthStateService.LoginAsync(result);

                var destination = "/dashboard";

                _NavigationManager.NavigateTo("/dashboard", forceLoad: false);
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