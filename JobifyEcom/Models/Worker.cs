using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a worker profile linked to a user account.
/// This profile contains information about the worker's skills,
/// job applications, and ratings received.
/// </summary>
public class Worker
{
    /// <summary>
    /// The unique identifier for this worker profile.
    /// <para>Automatically generated and cannot be modified externally.</para>
    /// </summary>
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// The foreign key linking this worker profile to the <see cref="Models.User"/> account.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property to the associated <see cref="Models.User"/> account.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The UTC timestamp when this worker profile was created.
    /// <para>Automatically generated and cannot be modified externally.</para>
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Collection of <see cref="Skill"/> objects representing the skills this worker possesses.
    /// </summary>
    public ICollection<Skill> Skills { get; set; } = [];

    /// <summary>
    /// Collection of <see cref="JobApplication"/> objects representing jobs
    /// that this worker has applied to.
    /// </summary>
    public ICollection<JobApplication> ApplicationsSubmitted { get; set; } = [];

    /// <summary>
    /// Collection of <see cref="Rating"/> objects representing ratings
    /// received by this worker from customers or clients.
    /// </summary>
    public ICollection<Rating> RatingsReceived { get; set; } = [];
}
