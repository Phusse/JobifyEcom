using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.DTOs.Workers;

/// <summary>
/// Represents the data required to verify or reject a worker's skill.
/// </summary>
public class VerifySkillRequest
{
	/// <summary>
	/// The verification status to assign to the skill.
	/// <para>For example: <see cref="VerificationStatus.Approved"/> or <see cref="VerificationStatus.Rejected"/>.</para>
	/// </summary>
	[Required(ErrorMessage = "Verification status is required.")]
	public required VerificationStatus Status { get; set; }

	/// <summary>
	/// Mandatory feedback or reasoning provided by the reviewer when updating the verification status.
	/// <para>Must be at least 5 characters and no more than 500 characters.</para>
	/// </summary>
	[Required(ErrorMessage = "A reviewer comment is required when verifying a skill.")]
	[MinLength(5, ErrorMessage = "Reviewer comment must be at least 5 characters long.")]
	[MaxLength(500, ErrorMessage = "Reviewer comment cannot exceed 500 characters.")]
	public required string ReviewerComment { get; set; }
}
