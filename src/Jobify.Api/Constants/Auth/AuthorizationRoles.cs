using Jobify.Domain.Enums;

namespace Jobify.Api.Constants.Auth;

public static class AuthorizationRoles
{
    public const string User = nameof(SystemRole.User);
    public const string Admin = nameof(SystemRole.Admin);
    public const string SuperAdmin = nameof(SystemRole.SuperAdmin);
}
