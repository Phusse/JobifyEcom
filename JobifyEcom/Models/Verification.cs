using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a verification record for any supported entity (e.g., job post, skill, worker profile).
/// </summary>
public class Verification
{
	/// <summary>
	/// The unique identifier for the verification record.
	/// This value is automatically set by the backend and cannot be modified externally.
	/// </summary>
	[Key]
	public Guid Id { get; private set; } = Guid.NewGuid();

	/// <summary>
	/// The type of entity being verified (e.g., JobPost, Skill, WorkerProfile).
	/// </summary>
	[Required]
	public EntityType EntityType { get; set; }

	/// <summary>
	/// The ID of the entity being verified.
	/// </summary>
	[Required]
	public Guid EntityId { get; set; }

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
	/// The ID of the user (admin) who reviewed this record.
	/// Null if not yet reviewed.
	/// </summary>
	public Guid? ReviewedByUserId { get; set; }

	/// <summary>
	/// The UTC datetime when the review was completed.
	/// Null if not yet reviewed.
	/// </summary>
	public DateTime? ReviewedAt { get; set; }

	/// <summary>
	/// Navigation to the admin user who reviewed this record (optional).
	/// </summary>
	public User? ReviewedByUser { get; set; }
}
