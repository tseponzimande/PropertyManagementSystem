namespace PropertyManagementSystem.UI.Components.Pages.UserProfile
{
    public partial class UserProfile
    {
        #region Dependencies

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

        private ChangePasswordRequestDto pwdModel = new();

        private string confirmPwd = string.Empty;

        private bool loading = true;

        private bool savingProfile = false;

        private bool Passwd = false;

        private string? profileError = null;

        private string? PasswdError = null;

        private Guid userId;

        private bool _dataLoaded = false;

        #endregion

        #region Lifecycle

        protected override async Task OnInitializedAsync()
        {
            var stored = await AuthState.GetCurrentUserAsync();

            if (stored is null)
                return;

            await LoadProfile(stored.UserId);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !_dataLoaded)
            {
                if (userId == Guid.Empty)
                {
                    var stored = await AuthState.GetCurrentUserAsync();

                    if (stored is null)
                    {
                        NavigationManager.NavigateTo("/login");
                        return;
                    }

                    await LoadProfile(stored.UserId);
                    StateHasChanged();
                }
            }
        }

        private async Task LoadProfile(Guid id)
        {
            _dataLoaded = true;

            userId = id;

            try
            {
                var dto = await UserService.GetUserByIdAsync(userId);
                if (dto is not null)
                    user = dto;
            }
            catch(Exception ex)
            {
                profileError = $"Error Occured : {ex.Message}";

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = profileError,
                    Detail = "Could Not Load Profile Info",
                    Duration = 5000
                });
            }
            finally
            {
                loading = false;
            }
        }

        #endregion

        #region Methods

        private async Task SaveProfile(UserDto dto)
        {
            savingProfile = true;

            profileError = null;

            try
            {
                await UserService.UpdateUserAsync(dto);
                NotificationService.Notify(NotificationSeverity.Success, "Saved", "Profile updated.");
            }
            catch (Exception ex)
            {
                profileError = ex.Message;
            }
            finally
            {
                savingProfile = false;
            }
        }

        private async Task SavePassword(ChangePasswordRequestDto dto)
        {
            Passwd = true;
            PasswdError = null;

            try
            {
                var ok = await AuthService.ChangePasswordAsync(userId, dto);
                if (ok)
                {
                    NotificationService.Notify(NotificationSeverity.Success, "Updated", "Password changed successfully.");
                    pwdModel = new();
                    confirmPwd = string.Empty;
                }
                else PasswdError = "Current password is incorrect.";
            }
            catch (Exception ex)
            {
                PasswdError = ex.Message;
            }
            finally
            {
                Passwd = false;
            }
        }

        #endregion
    }
}

