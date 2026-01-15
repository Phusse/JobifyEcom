using Jobify.Api.Constants.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Jobify.Api.Extensions.Claims;

public static class AuthorizationBuilderExtensions
{
    extension(AuthorizationBuilder builder)
    {
        public AuthorizationBuilder AddCustomPolicies()
        {
            builder.AddPolicy(
                AuthorizationPolicyNames.UserOnly,
                p => p.RequireRole(AuthorizationRoles.User)
            );

            builder.AddPolicy(
                AuthorizationPolicyNames.AdminOnly,
                p => p.RequireRole(AuthorizationRoles.Admin, AuthorizationRoles.SuperAdmin)
            );

            builder.AddPolicy(
                AuthorizationPolicyNames.SuperAdminOnly,
                p => p.RequireRole(AuthorizationRoles.SuperAdmin)
            );

            return builder;
        }
    }
}
