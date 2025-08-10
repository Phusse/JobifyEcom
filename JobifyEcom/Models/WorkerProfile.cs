using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a worker's profile, including bio, skills, rating, and associated user information.
/// </summary>
public class WorkerProfile
{
    /// <summary>
    /// The unique identifier for the worker profile.
    /// <br>This value is automatically set by the backend and cannot be modified externally.</br>
    /// </summary>
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated user account.
    /// </summary>
    [Required]
    public required Guid UserId { get; set; }

    /// <summary>
    /// The name of the worker.
    /// </summary>
    [Required]
    [MinLength(2)]
    [StringLength(100)]
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// The email address of the worker.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; } = string.Empty;

    /// <summary>
    /// A short biography or description of the worker.
    /// </summary>
    [StringLength(250)]
    public string? Bio { get; set; }

    /// <summary>
    /// The UTC date and time when the profile was created.
    /// <br>This value is automatically set by the backend and cannot be modified externally.</br>
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// The user entity associated with this profile.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// A collection of skills associated with this worker.
    /// </summary>
    public ICollection<Skill> Skills { get; set; } = [];

    /// <summary>
    /// A collection of job posts created by this worker.
    /// </summary>
    public ICollection<JobPost> JobPosts { get; set; } = [];
}
