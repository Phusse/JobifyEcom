namespace JobifyEcom.Contracts;

public static class ApiRoutes
{
    private const string root = "api";
    private const string version = "v1";


    private const string authController = "auth";
    public const string AuthBase = $"{root}/{version}/{authController}";

    public static class AuthPost
    {
        public const string Register = $"{AuthBase}/register";
        public const string Login = $"{AuthBase}/login";
        public const string ConfirmEmail = $"{AuthBase}/confirm-email";
    }

    public static class AuthGet
    {
        public const string Me = $"{AuthBase}/me";
    }

}
