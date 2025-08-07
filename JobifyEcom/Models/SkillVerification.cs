using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents the verification status and review details for a skill.
/// </summary>
public class SkillVerification
{
	/// <summary>
	/// The unique identifier for the verification record.
	/// </summary>
	[Key]
	public Guid Id { get; set; }

	/// <summary>
	/// The ID of the skill being verified.
	/// </summary>
	[Required]
	public Guid SkillId { get; set; }

	/// <summary>
	/// The current verification status.
	/// </summary>
	[Required]
	public VerificationStatus Status { get; set; } = VerificationStatus.Pending;

	/// <summary>
	/// Optional comment left by the reviewer explaining the decision.
	/// </summary>
	[StringLength(1000)]
	public string? ReviewerComment { get; set; }

	/// <summary>
	/// The ID of the user (admin) who reviewed the skill.
	/// Null if not yet reviewed.
	/// </summary>
	public Guid? ReviewedByUserId { get; set; }

	/// <summary>
	/// The UTC datetime when the review was completed.
	/// Null if not yet reviewed.
	/// </summary>
	public DateTime? ReviewedAt { get; set; }

	/// <summary>
	/// Navigation to the reviewed skill.
	/// </summary>
	public Skill Skill { get; set; } = null!;

	/// <summary>
	/// Navigation to the admin user who reviewed the skill (optional).
	/// </summary>
	public User? ReviewedByUser { get; set; }
}
