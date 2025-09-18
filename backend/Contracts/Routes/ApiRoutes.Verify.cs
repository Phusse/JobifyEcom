namespace JobifyEcom.Contracts.Routes;

internal static partial class ApiRoutes
{
	/// <summary>Routes related to verification operations.</summary>
	internal static partial class Verify
	{
		private const string Base = $"{Root}/{Version}/verify";

		internal static class Post
		{
			internal const string VerifySkill = $"{Base}/skill/{{id}}";
		}
	}
}
