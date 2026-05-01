namespace PropertyManagementSystem.UI.Dialogs
{
    public partial class EditProfileDialog
    {
        #region Dependencies

        [Inject]
        private Radzen.DialogService DialogService { get; set; } = null!;

        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        #endregion

        #region Parameters

        [Parameter]
        public UserDto User { get; set; } = new();

        #endregion

        #region Fields

        private UserDto model = new();
        private bool loading;
        private string? error;

        #endregion

        #region Lifecycle

        protected override void OnInitialized()
        {
            model = new UserDto
            {
                Id = User.Id,
                Name = User.Name,
                Email = User.Email,
                Role = User.Role
            };
        }

        #endregion

        #region Methods

        private async Task SaveProfile()
        {
            loading = true;
            error = null;

            try
            {
                await UserService.UpdateUserAsync(model);

                NotificationService.Notify(
                    NotificationSeverity.Success,
                    "Saved",
                    "Profile updated successfully"
                );

                DialogService.Close(model);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                loading = false;
            }
        }

        #endregion
    }
}