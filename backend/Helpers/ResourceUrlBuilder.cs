using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs.Jobs;
using JobifyEcom.DTOs.Workers;

namespace JobifyEcom.Helpers;

/// <summary>
/// Provides methods for building fully-qualified URLs to various resources in the application.
/// </summary>
internal static class ResourceUrlBuilder
{
	/// <summary>
	/// Builds the full URL to access the current worker's profile.
	/// </summary>
	/// <param name="request">The current HTTP request used to determine scheme and host.</param>
	/// <returns>The fully-qualified URL to the worker profile resource.</returns>
	internal static string BuildWorkerProfileResourceUrl(HttpRequest request)
	{
		string path = ApiRoutes.Worker.Get.Me;
		return $"{request.Scheme}://{request.Host}/{path}";
	}

	/// <summary>
	/// Builds the full URL to access a specific worker skill resource.
	/// </summary>
	/// <param name="request">The current HTTP request used to determine scheme and host.</param>
	/// <param name="data">The skill response data containing worker and skill IDs.</param>
	/// <returns>The fully-qualified URL to the skill resource, or an empty string if <paramref name="data"/> is null.</returns>
	internal static string BuildSkillResourceUrl(HttpRequest request, WorkerSkillResponse? data)
	{
		if (data is null) return string.Empty;

		string path = ApiRoutes.Worker.Get.SkillById
			.Replace("{{workerId}}", data.WorkerId.ToString())
			.Replace("{{skillId}}", data.Id.ToString());

		return $"{request.Scheme}://{request.Host}/{path}";
	}

	/// <summary>
	/// Builds the full URL to access a specific job resource.
	/// </summary>
	/// <param name="request">The current HTTP request used to determine scheme and host.</param>
	/// <param name="data">The job response data containing the job ID.</param>
	/// <returns>The fully-qualified URL to the job resource, or an empty string if <paramref name="data"/> is null.</returns>
	internal static string BuildJobResourceUrl(HttpRequest request, JobResponse? data)
	{
		if (data is null) return string.Empty;

		string path = ApiRoutes.Job.Get.ById
			.Replace("{{id}}", data.Id.ToString());

		return $"{request.Scheme}://{request.Host}/{path}";
	}

	/// <summary>
	/// Builds the full URL to access a specific job application resource.
	/// </summary>
	/// <param name="request">The current HTTP request used to determine scheme and host.</param>
	/// <param name="data">The job application response data containing job and application IDs.</param>
	/// <returns>The fully-qualified URL to the job application resource, or an empty string if <paramref name="data"/> is null.</returns>
	internal static string BuildJobApplicationResourceUrl(HttpRequest request, JobApplicationResponse? data)
	{
		if (data is null) return string.Empty;

		string path = ApiRoutes.Job.Get.ApplicationById
			.Replace("{{jobId}}", data.JobPostId.ToString())
			.Replace("{{applicationId}}", data.Id.ToString());

		return $"{request.Scheme}://{request.Host}/{path}";
	}
}
