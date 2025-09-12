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
    public static class User
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
        private const string Base = $"{Root}/{Version}/workers";
        private const string SkillsBase = $"{Base}/{{workerId}}/skills";

        /// <summary>GET endpoints for Worker.</summary>
        public static class Get
        {
            /// <summary>Get current authenticated worker profile.</summary>
            public const string Me = $"{Base}/me";

            /// <summary>Get a specific skill by ID (scoped under worker).</summary>
            public const string SkillById = $"{SkillsBase}/{{skillId}}";
        }

        /// <summary>POST endpoints for Worker.</summary>
        public static class Post
        {
            /// <summary>Create a worker profile for the current user.</summary>
            public const string CreateProfile = $"{Base}/me/create";

            /// <summary>Add a new skill to the current worker.</summary>
            public const string AddSkill = $"{Base}/me/skills";

            /// <summary>Verify a worker’s skill (admin only).</summary>
            public const string VerifySkill = $"{SkillsBase}/{{skillId}}/verify";
        }

        /// <summary>DELETE endpoints for Workers.</summary>
        public static class Delete
        {
            /// <summary>Delete the current worker profile.</summary>
            public const string Profile = $"{Base}/me/delete";

            /// <summary>Remove a skill from the current worker.</summary>
            public const string RemoveSkill = $"{Base}/me/skills/{{skillId}}";
        }
    }

    /// <summary>
    /// Routes related to Job operations.
    /// </summary>
    public static class Job
    {
        private const string Base = $"{Root}/{Version}/jobs";
        private const string ApplicationsBase = $"{Base}/{{jobId}}/applications";

        /// <summary>POST endpoints for Jobs.</summary>
        public static class Post
        {
            /// <summary>Create a new job.</summary>
            public const string Create = Base;

            /// <summary>Submit a job application for a specific job.</summary>
            public const string Apply = ApplicationsBase;
        }

        /// <summary>GET endpoints for Jobs.</summary>
        public static class Get
        {
            /// <summary>Get a job by ID.</summary>
            public const string ById = $"{Base}/{{id}}";

            /// <summary>Get a job application by ID (scoped under job).</summary>
            public const string ApplicationById = $"{ApplicationsBase}/{{applicationId}}";
        }

        /// <summary>PATCH endpoints for Jobs.</summary>
        public static class Patch
        {
            /// <summary>Update a job by ID.</summary>
            public const string Update = $"{Base}/{{id}}";

            /// <summary>Accept a job application.</summary>
            public const string AcceptApplication = $"{ApplicationsBase}/{{applicationId}}/accept";

            /// <summary>Reject a job application.</summary>
            public const string RejectApplication = $"{ApplicationsBase}/{{applicationId}}/reject";
        }

        /// <summary>DELETE endpoints for Jobs.</summary>
        public static class Delete
        {
            /// <summary>Delete a job by ID.</summary>
            public const string ById = $"{Base}/{{id}}";
        }
    }

    /// <summary>
    /// Routes related to Metadata (enums, lookup tables, etc.).
    /// </summary>
    public static class Metadata
    {
        private const string Base = $"{Root}/{Version}/metadata";

        /// <summary>GET endpoints for Metadata.</summary>
        public static class Get
        {
            /// <summary>Get all enums.</summary>
            public const string AllEnums = $"{Base}/enums";

            /// <summary>Get a specific enum type by its type name.</summary>
            public const string EnumByType = $"{Base}/enums/{{id}}";
        }
    }
}
