namespace PropertyManagementSystem.UI.Components.Shared
{
    public partial class ChatMessage
    {
        #region Parameters

        [Parameter]
        public MessageDto Message { get; set; } = new();

        [Parameter]
        public bool IsSent { get; set; }

        #endregion

        #region Styles

        private string BubbleStyle => string.Join(";", new[]
        {
            "padding:.55rem .8rem", "border-radius:1rem", IsSent
                ? "background:var(--rz-primary);color:#fff;border-bottom-right-radius:.25rem"
                : "background:var(--rz-base-700);border-bottom-left-radius:.25rem"
        });

        private string StatusStyle =>
            $"font-size:.85rem;color:{(Message.IsRead ? "var(--rz-info)" : "inherit")}";

        #endregion

        #region Values

        private string StatusIcon => Message.IsRead ? "done_all" : "done";

        private string TimeText => Message.CreatedAt.ToLocalTime().ToString("HH:mm");

        #endregion

        #region Helpers

        private static string FormatSize(long bytes) => bytes switch
        {
            < 1024 => $"{bytes} B",
            < 1024 * 1024 => $"{bytes / 1024} KB",
            _ => $"{bytes / (1024 * 1024):F1} MB"
        };

        #endregion
    }
}
