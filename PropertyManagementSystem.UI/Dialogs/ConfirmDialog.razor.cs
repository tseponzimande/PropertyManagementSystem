namespace PropertyManagementSystem.UI.Dialogs
{
    public partial class ConfirmDialog
    {
        #region Dependencies

        [Inject]
        private DialogService DialogService { get; set; } = default!;

        #endregion

        #region Parameters

        [Parameter]
        public string Message { get; set; } = "Are you sure?";

        [Parameter] 
        public string SubMessage { get; set; } = string.Empty;

        [Parameter]
        public string ConfirmText { get; set; } = "Confirm";

        [Parameter]
        public string ConfirmIcon { get; set; } = "check";

        [Parameter] 
        public ButtonStyle ConfirmStyle { get; set; } = ButtonStyle.Danger;

        [Parameter] 
        public string Icon { get; set; } = "warning";

        [Parameter]
        public string IconColor { get; set; } = "danger";

        #endregion
    }
}
