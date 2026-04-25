namespace PropertyManagementSystem.Application.Jwt
{
    public class JwtSettings
    {
        #region Properties and fields

        public string Key { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int DurationInMinutes { get; set; }

        #endregion
    }
}
