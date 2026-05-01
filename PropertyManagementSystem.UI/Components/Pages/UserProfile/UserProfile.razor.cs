namespace PropertyManagementSystem.UI.Components.Pages.UserProfile
{
    public partial class UserProfile
    {
        #region Dependencies

        [Inject]
        private Radzen.DialogService DialogService { get; set; } = null!;

        [Inject] 
        private IUserService UserService { get; set; } = null!;

        [Inject] 
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private AuthStateService AuthState { get; set; } = null!;

        [Inject] 
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        #endregion

        #region Fields

        private UserDto user = new();

        private bool isLoading = true;

        private string? profileError;

        private Guid userId;

        #endregion

        #region Lifecycle

        protected override async Task OnInitializedAsync()
        {
            var stored = await AuthState.GetCurrentUserAsync();

            if (stored is null)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }

            await LoadProfile(stored.UserId);
        }

        #endregion

        #region Methods

        private async Task LoadProfile(Guid id)
        {
            userId = id;

            try
            {
                var dto = await UserService.GetUserByIdAsync(userId);
                if (dto is not null)
                    user = dto;
            }
            catch (Exception ex)
            {
                profileError = $"Error Occurred: {ex.Message}";

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = "Could not load profile info",
                    Duration = 5000
                });
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task OpenChangePasswordDialog()
        {
            await DialogService.OpenAsync<ChangePasswordDialog>(
                "Change Password",
                new Dictionary<string, object>
                {
                    { "UserId", userId }
                },
                new Radzen.DialogOptions
                {
                    Width = "450px",
                    CloseDialogOnOverlayClick = true
                });
        }

        private async Task OpenEditProfileDialog()
        {
            var result = await DialogService.OpenAsync<EditProfileDialog>(
                "Edit Profile",
                new Dictionary<string, object>
                {
                    { "User", user }
                },
                new Radzen.DialogOptions
                {
                    Width = "500px",
                    CloseDialogOnOverlayClick = true
                });

            if (result is UserDto updatedUser)
            {
                user = updatedUser;
            }
        }

        #endregion
    }
}