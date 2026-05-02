namespace PropertyManagementSystem.UI.Components.Shared
{
    public partial class EmptyState
    {
        #region Parameter

        [Parameter]
        public string Message { get; set; } = "No data found";

        [Parameter]
        public string Icon { get; set; } = "inbox";

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        #endregion
    }
}
