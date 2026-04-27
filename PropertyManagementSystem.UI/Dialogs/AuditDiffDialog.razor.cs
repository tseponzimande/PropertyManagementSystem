namespace PropertyManagementSystem.UI.Dialogs
{
    public partial class AuditDiffDialog
    {
        #region

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        #endregion

        #region Parameter

        [Parameter]
        public AuditLogDto Log { get; set; } = new();

        #endregion

        #region Methods

        private static string PrettyJson(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                
                return string.Empty;
            try
            {
                var obj = System.Text.Json.JsonSerializer.Deserialize<object>(raw);
                return System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return raw;
            }
        }

        #endregion
    }
}
