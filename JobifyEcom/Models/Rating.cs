using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a customer-submitted rating for a worker.
/// </summary>
public class Rating
{
	/// <summary>
	/// The unique identifier for the rating.
	/// <br>This value is automatically set by the backend and cannot be modified externally.</br>
	/// </summary>
	[Key]
	public Guid Id { get; private set; } = Guid.NewGuid();

	/// <summary>
	/// The rating score (e.g., 1–5 stars).
	/// </summary>
	[Required]
	[Range(1, 5)]
	public required int Score { get; set; }

	/// <summary>
	/// Optional feedback from the customer.
	/// </summary>
	[StringLength(1000)]
	public string? Comment { get; set; }

	/// <summary>
	/// The date and time when the rating was submitted.
	/// <br>This value is automatically set by the backend and cannot be modified externally.</br>
	/// </summary>
	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

	/// <summary>
	/// The ID of the customer who submitted the rating.
	/// </summary>
	[Required]
	public required Guid CustomerId { get; set; }

	/// <summary>
	/// The ID of the worker profile being rated.
	/// </summary>
	[Required]
	public required Guid WorkerProfileId { get; set; }

	/// <summary>
	/// The job this rating is associated with.
	/// </summary>
	public Guid? JobPostId { get; set; }

	/// <summary>
	/// The customer who submitted the rating.
	/// </summary>
	public User? Customer { get; set; }

	/// <summary>
	/// The worker profile that was rated.
	/// </summary>
	public WorkerProfile? WorkerProfile { get; set; }

	/// <summary>
	/// The job post related to the rating, if any.
	/// </summary>
	public JobPost? JobPost { get; set; }
}
