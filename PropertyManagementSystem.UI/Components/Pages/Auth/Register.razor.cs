using Microsoft.AspNetCore.Components.Forms;

namespace PropertyManagementSystem.UI.Components.Pages.Auth
{
    public partial class Register
    {
        [Inject]
        private IAuthService _AuthService { get; set; } = null!;

        [Inject]
        private AuthStateService _AuthStateService { get; set; } = null!;

        [Inject]
        private NavigationManager _NavigationManager { get; set; } = null!;


        private readonly RegisterRequestDto model = new();
        private bool isLoading = false;
        private string? errorMessage = null;

        private readonly List<string> roles = new() { "Owner", "Tenant" };


        private EditContext editContext;

        protected override void OnInitialized()
        {
            editContext = new EditContext(model);
        }

        //private async Task UserRegister(RegisterRequestDto submitted)
        private async Task UserRegister()
        {
            isLoading = true;
            errorMessage = null;

            try
            {
                var result = await _AuthService.RegisterAsync(model, string.Empty, null);

                if (result is null)
                {
                    errorMessage = "Registration failed. The email may already be in use.";
                    return;
                }

                await _AuthStateService.LoginAsync(result);
                _NavigationManager.NavigateTo("/dashboard");
               
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message.Contains("Email already exists", StringComparison.OrdinalIgnoreCase)
                    ? "An account with that email already exists."
                    : "An unexpected error occurred.";
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
