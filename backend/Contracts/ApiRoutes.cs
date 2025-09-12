namespace JobifyEcom.Contracts;

/// <summary>
/// Defines all static API route paths used in the application.
/// Organized by domain (Auth, User, Worker, Job, Metadata)
/// and HTTP verbs (GET, POST, PATCH, DELETE).
/// </summary>
internal static class ApiRoutes
{
    private const string Root = "api";
    private const string Version = "v1";

    /// <summary>Routes related to authentication operations.</summary>
    internal static class Auth
    {
        private const string Base = $"{Root}/{Version}/auth";

        internal static class Post
        {
            internal const string Register = $"{Base}/register";
            internal const string Login = $"{Base}/login";
            internal const string Refresh = $"{Base}/refresh";
        }

        internal static class Patch
        {
            internal const string Logout = $"{Base}/logout";
        }
    }

    /// <summary>Routes related to user management.</summary>
    internal static class User
    {
        private const string Base = $"{Root}/{Version}/users";

        internal static class Get
        {
            internal const string Me = $"{Base}/me";
            internal const string ById = $"{Base}/{{id}}";
            internal const string All = Base;
        }

        internal static class Post
        {
            internal const string PasswordResetRequest = $"{Base}/{{id}}/password-reset/request";
            internal const string PasswordResetConfirm = $"{Base}/{{id}}/password-reset/confirm";
        }

        internal static class Patch
        {
            internal const string ConfirmEmail = $"{Base}/confirm-email";
            internal const string Me = $"{Base}/me";
            internal const string Lock = $"{Base}/{{id}}/lock";
            internal const string Unlock = $"{Base}/{{id}}/unlock";
        }

        internal static class Delete
        {
            internal const string Me = $"{Base}/me";
            internal const string ById = $"{Base}/{{id}}";
        }
    }

    /// <summary>Routes related to worker operations.</summary>
    internal static class Worker
    {
        private const string Base = $"{Root}/{Version}/workers";
        private const string SkillsBase = $"{Base}/{{workerId}}/skills";

        internal static class Get
        {
            internal const string Me = $"{Base}/me";
            internal const string SkillById = $"{SkillsBase}/{{skillId}}";
        }

        internal static class Post
        {
            internal const string Create = $"{Base}/me";
            internal const string AddSkill = $"{Base}/me/skills";
            internal const string VerifySkill = $"{SkillsBase}/{{skillId}}/verify";
        }

        internal static class Delete
        {
            internal const string Me = $"{Base}/me";
            internal const string RemoveSkill = $"{Base}/me/skills/{{skillId}}";
        }
    }

    /// <summary>Routes related to job operations.</summary>
    internal static class Job
    {
        private const string Base = $"{Root}/{Version}/jobs";
        private const string ApplicationsBase = $"{Base}/{{jobId}}/applications";

        internal static class Post
        {
            internal const string Create = Base;
            internal const string Apply = ApplicationsBase;
        }

        internal static class Get
        {
            internal const string ById = $"{Base}/{{id}}";
            internal const string ApplicationById = $"{ApplicationsBase}/{{applicationId}}";
        }

        internal static class Patch
        {
            internal const string Update = $"{Base}/{{id}}";
            internal const string AcceptApplication = $"{ApplicationsBase}/{{applicationId}}/accept";
            internal const string RejectApplication = $"{ApplicationsBase}/{{applicationId}}/reject";
        }

        internal static class Delete
        {
            internal const string ById = $"{Base}/{{id}}";
        }
    }

    /// <summary>Routes related to metadata (enums, lookups, etc.).</summary>
    internal static class Metadata
    {
        private const string Base = $"{Root}/{Version}/metadata";

        internal static class Get
        {
            internal const string AllEnums = $"{Base}/enums";
            internal const string EnumByType = $"{Base}/enums/{{id}}";
        }
    }
}
