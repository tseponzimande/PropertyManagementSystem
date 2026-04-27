namespace PropertyManagementSystem.UI.Components.Layout
{
    public partial class NavMenu
    {
        #region Dependencies

        [Inject]
        private AuthStateService AuthStateService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        #endregion

        #region LifeCycle Methods

        private async Task Logout()
        {
            await AuthStateService.LogoutAsync();
            Navigation.NavigateTo("/login", true);
        }

        #endregion
    }
}
