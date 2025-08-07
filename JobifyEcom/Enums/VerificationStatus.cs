using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Enums;

/// <summary>
/// Indicates the current verification state of a skill.
/// </summary>
public enum VerificationStatus
{
	/// <summary>
	/// The skill has been submitted and is awaiting review.
	/// </summary>
	[Display(Name = "Pending Review")]
	Pending,

	/// <summary>
	/// The skill has been reviewed and approved by an admin.
	/// </summary>
	[Display(Name = "Approved")]
	Approved,

	/// <summary>
	/// The skill was reviewed but rejected by an admin.
	/// </summary>
	[Display(Name = "Rejected")]
	Rejected,
}
