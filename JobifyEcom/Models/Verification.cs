using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a verification record for any supported entity in the system.
/// </summary>
public class Verification
{
	/// <summary>
	/// The unique identifier for this verification record.
	/// <para>Automatically generated and cannot be modified externally.</para>
	/// </summary>
	[Key]
	public Guid Id { get; private set; } = Guid.NewGuid();

	/// <summary>
	/// The type of entity being verified. See <see cref="Enums.EntityType"/>.
	/// </summary>
	[Required]
	public required EntityType EntityType { get; set; }

	/// <summary>
	/// The unique identifier of the entity being verified.
	/// </summary>
	[Required]
	public required Guid EntityId { get; set; }

	/// <summary>
	/// The current status of the verification.
	/// Defaults to <see cref="VerificationStatus.Pending"/>.
	/// </summary>
	[Required]
	public required VerificationStatus Status { get; set; } = VerificationStatus.Pending;

	/// <summary>
	/// Optional comment left by the reviewer explaining their decision.
	/// </summary>
	[StringLength(1000)]
	public string? ReviewerComment { get; set; }

	/// <summary>
	/// The unique identifier of the admin user who reviewed this record.
	/// <c>Null</c> if the verification has not yet been reviewed.
	/// </summary>
	public Guid? ReviewerId { get; set; }

	/// <summary>
	/// Navigation property to the admin user who reviewed this record.
	/// <c>Null</c> if not yet reviewed.
	/// </summary>
	public User? Reviewer { get; set; }

	/// <summary>
	/// The UTC datetime when the verification was completed.
	/// <c>Null</c> if the verification has not yet been reviewed.
	/// </summary>
	public DateTime? ReviewedAt { get; set; }
}
