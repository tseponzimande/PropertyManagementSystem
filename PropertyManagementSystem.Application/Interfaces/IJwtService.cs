namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user, List<string> roles);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
