namespace JobifyEcom.Contracts.Routes;

internal static partial class ApiRoutes
{
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
}
