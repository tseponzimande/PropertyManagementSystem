namespace PropertyManagementSystem.UI.Dialogs
{
    public partial class ChangePasswordDialog
    {
        #region Dependencies

        [Inject]
        private Radzen.DialogService DialogService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Parameter]
        public Guid UserId { get; set; }

        #endregion

        #region Parameters

        private ChangePasswordRequestDto pwdModel = new();
        private bool loading;
        private string? error;

        #endregion

        #region Methods

        private async Task SavePassword()
        {
            loading = true;
            error = null;

            try
            {
                var ok = await AuthService.ChangePasswordAsync(UserId, pwdModel);

                if (ok)
                {
                    NotificationService.Notify(NotificationSeverity.Success, "Success", "Password updated");
                    DialogService.Close(true);
                }
                else
                {
                    error = "Current password is incorrect.";
                }
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
