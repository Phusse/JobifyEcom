namespace Jobify.Api.Constants.Routes;

internal static partial class ApiRoutes
{
    public static class User
    {
        public const string Base = $"{BasePath}/users";

        public const string GetById = "/{id:guid}";
    }
}
