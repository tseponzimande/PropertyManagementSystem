namespace PropertyManagementSystem.UI.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : Guid.Empty;
    }
    public static string GetName(this ClaimsPrincipal user)
    {
        var name = user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue("name") ?? "User";

        return name;
    }
    public static string GetEmail(this ClaimsPrincipal user)
    {
        var email = user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue("email") ?? string.Empty;

        return email;
    }
    public static string GetRole(this ClaimsPrincipal user)
    {
        var role = user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        return role;
    }
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole(RoleEnum.Admin.ToString());
    }
    public static bool IsOwner(this ClaimsPrincipal user)
    {
        return user.IsInRole(RoleEnum.Owner.ToString());
    }
    public static bool IsTenant(this ClaimsPrincipal user)
    {
        return user.IsInRole(RoleEnum.Tenant.ToString());
    }
    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated == true;
    }
}