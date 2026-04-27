namespace PropertyManagementSystem.UI.Service
{
    public class AuthStateService
    {
        private readonly JwtAuthenticationStateProvider _authProvider;
        private readonly ProtectedLocalStorage _storage;

        private const string UserKey = "authUser";

        public AuthStateService(JwtAuthenticationStateProvider authProvider,ProtectedLocalStorage storage)
        {
            _authProvider = authProvider;
            _storage = storage;
        }

        public async Task LoginAsync(LoginResponseDto response)
        {
            await _storage.SetAsync(UserKey, response);
            await _authProvider.LoginAsync(response.AccessToken);
        }

        public async Task LogoutAsync()
        {
            await _storage.DeleteAsync(UserKey);
            await _authProvider.LogoutAsync();
        }

        public async Task<LoginResponseDto?> GetCurrentUserAsync()
        {
            try
            {
                var result = await _storage.GetAsync<LoginResponseDto>(UserKey);
                return result.Success ? result.Value : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ClaimsPrincipal> GetClaimsPrincipalAsync()
        {
            var state = await _authProvider.GetAuthenticationStateAsync();
            return state.User;
        }
        public void MarkInteractive()
        {
            _authProvider.MarkAppAsInteractive();
        }

        public async Task<string?> GetTokenAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.AccessToken;
        }
    }
}
