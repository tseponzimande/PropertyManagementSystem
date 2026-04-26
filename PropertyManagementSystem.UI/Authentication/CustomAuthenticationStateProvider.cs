namespace PropertyManagementSystem.UI.Authentication;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _storage;
    private bool _isInteractive;

    private const string TokenKey = "authToken";

    public JwtAuthenticationStateProvider(ProtectedLocalStorage storage)
    {
        _storage = storage;
    }

    public void MarkAppAsInteractive()
    {
        _isInteractive = true;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!_isInteractive)
            return Anonymous();

        try
        {
            var tokenResult = await _storage.GetAsync<string>(TokenKey);

            if (!tokenResult.Success || string.IsNullOrWhiteSpace(tokenResult.Value))
                return Anonymous();

            var identity = BuildIdentity(tokenResult.Value);

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return Anonymous();
        }
    }

    public async Task LoginAsync(string token)
    {
        await _storage.SetAsync(TokenKey, token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        await _storage.DeleteAsync(TokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous()));
    }

    private static ClaimsIdentity BuildIdentity(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        var claims = token.Claims.Select(c =>
        {
            if (c.Type.Equals("role", StringComparison.OrdinalIgnoreCase) ||
                c.Type == ClaimTypes.Role)
            {
                return new Claim(ClaimTypes.Role, c.Value);
            }
            return c;
        });

        return new ClaimsIdentity(claims, authenticationType: "jwt");
    }

    private static AuthenticationState Anonymous()
    {
        return new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity()));
    }
}
