using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Enums;

/// <summary>
/// Represents the status of a job application submitted by a worker for a job post.
/// </summary>
public enum JobApplicationStatus
{
	/// <summary>
	/// The application is awaiting review or action.
	/// </summary>
	[Display(Name = "Pending")]
	Pending,

	/// <summary>
	/// The application has been accepted by the customer.
	/// </summary>
	[Display(Name = "Accepted")]
	Accepted,

	/// <summary>
	/// The application has been rejected by the customer.
	/// </summary>
	[Display(Name = "Rejected")]
	Rejected,
}
