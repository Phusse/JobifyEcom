namespace Jobify.Api.Constants.Routes;

internal static partial class ApiRoutes
{
    public static class Auth
    {
        public const string Base = $"{ApiBasePath}/auth";

        public const string Register = "/register";
        public const string Login = "/login";
        public const string Refresh = "/refresh";
        public const string Logout = "/logout";
    }
}
