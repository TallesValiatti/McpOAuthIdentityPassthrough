using Cars.Mcp.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Cars.Mcp.Extensions;

public static class AuthorizationBuilderExtensions
{
    public static void AddCarsPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy("McpAccess", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireAssertion(context =>
                context.User.HasAnyScope(
                    CarsScopes.Read,
                    CarsScopes.Write,
                    CarsScopes.Delete));
        });

        options.AddPolicy("CarsRead", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireAssertion(context =>
                context.User.HasScope(CarsScopes.Read));
        });

        options.AddPolicy("CarsWrite", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireAssertion(context =>
                context.User.HasScope(CarsScopes.Write));
        });

        options.AddPolicy("CarsDelete", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireAssertion(context =>
                context.User.HasScope(CarsScopes.Delete));
        });
    }
}
