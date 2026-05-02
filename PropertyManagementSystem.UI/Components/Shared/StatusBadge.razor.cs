namespace PropertyManagementSystem.UI.Components.Shared
{
    public partial class StatusBadge
    {
        #region Parameters

        [Parameter]
        public string Status { get; set; } = string.Empty;

        #endregion

        #region Methods

        private string Display => Status switch
        {
            "InProgress" => "In Progress",
            _ => Status
        };

        public BadgeStyle Style => Status?.ToLowerInvariant() switch
        {
            #region Generic

            "approved" => BadgeStyle.Success,
            "pending" => BadgeStyle.Warning,
            "reject" => BadgeStyle.Danger,

            #endregion

            #region Unit

            "available" => BadgeStyle.Success,
            "occupied" => BadgeStyle.Info,

            #endregion

            #region Lease

            "active" => BadgeStyle.Success,
            "expired" => BadgeStyle.Danger,
            "terminated" => BadgeStyle.Light,

            #endregion

            #region Maintenance

            "open" => BadgeStyle.Danger,
            "inprogress" => BadgeStyle.Warning,
            "resolved" => BadgeStyle.Success,

            #endregion

            #region Payment
            "completed" => BadgeStyle.Success,
            _ => BadgeStyle.Light

            #endregion
        };

        #endregion
    }
}
