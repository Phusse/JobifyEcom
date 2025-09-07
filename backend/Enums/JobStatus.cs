using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Enums;

/// <summary>
/// Represents the current lifecycle stage of a job post within the platform.
/// This status is independent of verification and reflects whether the job is open for applications,
/// closed to new applicants, or fully completed.
/// </summary>
public enum JobStatus
{
	/// <summary>
	/// The job post is visible and open for applications.
	/// </summary>
	[Display(Name = "Open")]
	Open,

	/// <summary>
	/// The job post is no longer accepting applications.
	/// This may occur when the employer has chosen a candidate or decided not to proceed.
	/// </summary>
	[Display(Name = "Closed")]
	Closed,

	/// <summary>
	/// The job has been completed by the assigned worker or team.
	/// </summary>
	[Display(Name = "Completed")]
	Completed
}
