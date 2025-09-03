namespace JobifyEcom.Services;

/// <summary>
/// Provides a unified entry point for all job-related operations,
/// grouping together services for job management and job applications.
/// </summary>
public interface IJobDomainService
{
	/// <summary>
	/// Gets the service responsible for creating, updating, retrieving,
	/// and deleting job posts.
	/// </summary>
	IJobService JobService { get; }

	/// <summary>
	/// Gets the service responsible for managing job applications,
	/// including creation, retrieval, and status updates.
	/// </summary>
	IJobApplicationService JobApplicationService { get; }
}
