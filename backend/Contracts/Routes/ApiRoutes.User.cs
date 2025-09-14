namespace JobifyEcom.Contracts.Routes;

internal static partial class ApiRoutes
{
	/// <summary>Routes related to user management.</summary>
	internal static class User
	{
		private const string Base = $"{Root}/{Version}/users";

		internal static class Get
		{
			internal const string Me = $"{Base}/me";
			internal const string ById = $"{Base}/{{id}}";
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
}
