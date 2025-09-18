namespace JobifyEcom.Contracts.Routes;

internal static partial class ApiRoutes
{
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
}
