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

            /// <summary>Refresh authentication tokens.</summary>
            public const string Refresh = $"{Base}/refresh";
        }

        /// <summary>PATCH endpoints for Auth.</summary>
        public static class Patch
        {
            /// <summary>Logout a user.</summary>
            public const string Logout = $"{Base}/logout";
        }
    }

    /// <summary>
    /// Routes related to User Management operations.
    /// </summary>
    public static class Users
    {
        private const string Base = $"{Root}/{Version}/users";

        /// <summary>GET endpoints for Users.</summary>
        public static class Get
        {
            /// <summary>Get current authenticated user profile.</summary>
            public const string Me = $"{Base}/me";

            /// <summary>Get public user profile by ID.</summary>
            public const string ById = $"{Base}/{{id}}";

            /// <summary>list/search users with paging and filtering.</summary>
            public const string List = $"{Base}";
        }

        /// <summary>POST endpoints for Users.</summary>
        public static class Post
        {
            /// <summary>Request password reset.</summary>
            public const string PasswordResetRequest = $"{Base}/{{id}}/password-reset/request";

            /// <summary>Reset password with token.</summary>
            public const string PasswordResetConfirm = $"{Base}/{{id}}/password-reset/confirm";
        }

        /// <summary>PATCH endpoints for Users.</summary>
        public static class Patch
        {
            /// <summary>Confirm email address via token.</summary>
            public const string ConfirmEmail = $"{Base}/confirm-email";

            /// <summary>Update user profile.</summary>
            public const string Update = $"{Base}/me/update";

            /// <summary>Lock user account.</summary>
            public const string Lock = $"{Base}/{{id}}/lock";

            /// <summary>Unlock user account.</summary>
            public const string Unlock = $"{Base}/{{id}}/unlock";
        }

        /// <summary>DELETE endpoints for Users.</summary>
        public static class Delete
        {
            /// <summary>Delete current authenticated user profile.</summary>
            public const string Me = $"{Base}/me/delete";

            /// <summary>Delete user profile.</summary>
            public const string ById = $"{Base}/{{id}}";
        }
    }

    /// <summary>
    /// Routes related to Worker operations.
    /// </summary>
    public static class Worker
    {
        private const string Base = $"{Root}/{Version}/worker";

        /// <summary>GET endpoints for Worker.</summary>
        public static class Get
        {
            /// <summary>Get current authenticated worker profile.</summary>
            public const string Me = $"{Base}/me";

            /// <summary>Get worker profile by ID (public view).</summary>
            public const string ById = $"{Base}/{{id}}";
        }

        /// <summary>POST endpoints for Worker.</summary>
        public static class Post
        {
            /// <summary>Create a worker profile for the current user.</summary>
            public const string CreateProfile = $"{Base}/me/create";
        }

        /// <summary>DELETE endpoints for Workers.</summary>
        public static class Delete
        {
            /// <summary>Delete the current worker profile.</summary>
            public const string Profile = $"{Base}/me/delete";
        }
    }
}
