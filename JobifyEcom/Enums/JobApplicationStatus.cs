using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Enums;

/// <summary>
/// Represents the status of a job application submitted by a customer for a job post.
/// </summary>
public enum JobApplicationStatus
{
	/// <summary>
	/// The application is awaiting review or action.
	/// </summary>
	[Display(Name = "Pending")]
	Pending,

	/// <summary>
	/// The application has been accepted by the worker or system.
	/// </summary>
	[Display(Name = "Accepted")]
	Accepted,

	/// <summary>
	/// The application has been rejected by the worker or system.
	/// </summary>
	[Display(Name = "Rejected")]
	Rejected
}
