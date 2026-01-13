namespace Jobify.Api.Constants.Routes;

internal static partial class ApiRoutes
{
    public static class User
    {
        public const string Base = $"{ApiBasePath}/users";

        public const string GetById = "/{id:guid}";
        public const string Me = "/me";
    }
}
