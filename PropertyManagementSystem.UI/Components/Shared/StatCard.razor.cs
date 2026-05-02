namespace PropertyManagementSystem.UI.Components.Shared
{
    public partial class StatCard
    {
        #region Parameters

        [Parameter]
        public string Title { get; set; } = "";

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string Icon { get; set; } = "info";

        [Parameter]
        public string Color { get; set; } = "primary";

        [Parameter]
        public string? SubText { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        #endregion
    }
}
