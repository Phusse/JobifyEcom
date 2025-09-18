namespace JobifyEcom.Contracts.Routes;

internal static partial class ApiRoutes
{
	/// <summary>Routes related to search operations.</summary>
	internal static class Search
	{
		private const string Base = $"{Root}/{Version}/search";

		internal static class Get
		{
			internal const string Users = $"{Base}/users";
		}
	}
}
