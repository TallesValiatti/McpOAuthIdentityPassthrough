using System.Security.Claims;

namespace Cars.Mcp.Auth;

public static class ClaimsPrincipalExtensions
{
    public static bool HasScope(this ClaimsPrincipal user, string requiredScope)
    {
        var scopeClaim =
            user.FindFirst("scp")?.Value ??
            user.FindFirst("http://schemas.microsoft.com/identity/claims/scope")?.Value;

        if (string.IsNullOrWhiteSpace(scopeClaim))
            return false;

        var scopes = scopeClaim.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return scopes.Contains(requiredScope, StringComparer.OrdinalIgnoreCase);
    }

    public static bool HasAnyScope(this ClaimsPrincipal user, params string[] requiredScopes)
    {
        return requiredScopes.Any(user.HasScope);
    }
}
