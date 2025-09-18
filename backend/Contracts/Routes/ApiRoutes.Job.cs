namespace JobifyEcom.Contracts.Routes;

internal static partial class ApiRoutes
{
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
}
