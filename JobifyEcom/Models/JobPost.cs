using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a job posted by a <see cref="User"/>.
/// Workers can apply to this job, and optionally leave ratings after completion.
/// </summary>
public class JobPost
{
    /// <summary>
    /// The unique identifier for this job post.
	/// <para>Automatically generated and cannot be modified externally.</para>
    /// </summary>
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the user who posted this job.
    /// </summary>
    [Required]
    public Guid PostedByUserId { get; set; }

    /// <summary>
    /// Navigation property to the <see cref="User"/> who posted this job.
    /// </summary>
    public User PostedBy { get; set; } = null!;

    /// <summary>
    /// The title of the job post.
    /// Must be between 5 and 100 characters.
    /// </summary>
    [Required, MinLength(5), StringLength(100)]
    public required string Title { get; set; } = string.Empty;

    /// <summary>
    /// The detailed description of the job.
    /// Must be between 10 and 2000 characters.
    /// </summary>
    [Required, MinLength(10), StringLength(2000)]
    public required string Description { get; set; } = string.Empty;

    /// <summary>
    /// The price offered for completing the job.
    /// Must be between 0.01 and 1,000,000.
    /// </summary>
    [Required, Range(0.01, 1000000)]
    public required decimal Price { get; set; }

    /// <summary>
    /// The current status of the job <see cref="JobStatus"/>.
    /// </summary>
    [Required]
    public required JobStatus Status { get; set; } = JobStatus.Open;

    /// <summary>
    /// The UTC timestamp when this job was created.
	/// <para>Automatically generated and cannot be modified externally.</para>
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// The collection of applications submitted to this job.
    /// </summary>
    public ICollection<JobApplication> ApplicationsReceived { get; set; } = [];

    /// <summary>
    /// The collection of ratings received for this job.
    /// </summary>
    public ICollection<Rating> RatingsReceived { get; set; } = [];
}
