namespace JobifyEcom.Contracts.Routes;

internal static partial class ApiRoutes
{
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
