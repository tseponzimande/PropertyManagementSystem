namespace PropertyManagementSystem.UI.Components.Shared
{
    public partial class LoadingOverlay
    {
        #region Depedencies

        [Inject]
        private LoadingService Loader { get; set; } = null!;

        #endregion

        #region Methods

        protected override void OnInitialized()
        => Loader.OnChange += StateHasChanged;

        public void Dispose()
            => Loader.OnChange -= StateHasChanged;

        #endregion
    }
}
