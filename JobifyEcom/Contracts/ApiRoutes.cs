namespace JobifyEcom.Contracts;

/// <summary>
/// Defines all static API route paths used in the application.
/// Organizes them by domain (Auth, Job, Worker),
/// and by HTTP verbs (Get, Post, Put, Patch, Delete).
/// </summary>
public static class ApiRoutes
{
    private const string Root = "api";
    private const string Version = "v1";

    /// <summary>
    /// Routes related to Auth operations.
    /// </summary>
    public static class Auth
    {
        private const string Base = $"{Root}/{Version}/auth";

        /// <summary>POST endpoints for Auth.</summary>
        public static class Post
        {
            /// <summary>Register a new user.</summary>
            public const string Register = $"{Base}/register";

            /// <summary>Login a user.</summary>
            public const string Login = $"{Base}/login";
        }

        /// <summary>PATCH endpoints for Auth.</summary>
        public static class Patch
        {
            /// <summary>Logout a user.</summary>
            public const string Logout = $"{Base}/logout";
        }
    }
}
