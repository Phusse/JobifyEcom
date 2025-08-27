using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a rating given by a <see cref="User"/> (Reviewer) to a <see cref="Models.Worker"/>.
/// Optionally linked to a specific <see cref="Models.Job"/>.
/// </summary>
public class Rating
{
	/// <summary>
	/// The unique identifier for this rating.
	/// <para>Automatically generated and cannot be modified externally.</para>
	/// </summary>
	[Key]
	public Guid Id { get; private set; } = Guid.NewGuid();

	/// <summary>
	/// The score given in this rating, from 1 (lowest) to 5 (highest).
	/// </summary>
	[Required, Range(1, 5)]
	public required int Score { get; set; }

	/// <summary>
	/// Optional textual comment providing more context about the rating.
	/// </summary>
	[StringLength(1000)]
	public string? Comment { get; set; }

	/// <summary>
	/// The UTC timestamp when this rating was created.
	/// <para>Automatically generated and cannot be modified externally.</para>
	/// </summary>
	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

	/// <summary>
	/// The ID of the user who submitted this rating.
	/// </summary>
	[Required]
	public Guid ReviewerId { get; set; }

	/// <summary>
	/// Navigation property to the <see cref="User"/> who submitted this rating.
	/// </summary>
	public User? Reviewer { get; set; }

	/// <summary>
	/// The ID of the <see cref="Models.Worker"/> being rated.
	/// </summary>
	[Required]
	public Guid WorkerProfileId { get; set; }

	/// <summary>
	/// Navigation property to the <see cref="Models.Worker"/> being rated.
	/// </summary>
	public Worker? Worker { get; set; }

	/// <summary>
	/// Optional ID of the <see cref="Models.Job"/> associated with this rating.
	/// </summary>
	public Guid? JobPostId { get; set; }

	/// <summary>
	/// Navigation property to the optional <see cref="Models.Job"/> associated with this rating.
	/// </summary>
	public Job? Job { get; set; }
}
