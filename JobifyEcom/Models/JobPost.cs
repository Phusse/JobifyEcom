using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a job or service offering created by a worker.
/// </summary>
public class JobPost
{
    /// <summary>
    /// The unique identifier for the job post.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the worker who created the job post.
    /// </summary>
    [Required]
    public Guid WorkerId { get; set; }

    /// <summary>
    /// The title of the job post.
    /// </summary>
    [Required]
    [MinLength(5)]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// A detailed description of the job or service.
    /// </summary>
    [Required]
    [MinLength(10)]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The price or cost for the job.
    /// </summary>
    [Required]
    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    /// <summary>
    /// The current status of the job post (e.g., Available, Booked, Completed).
    /// </summary>
    [Required]
    public JobStatus Status { get; set; } = JobStatus.Available;

    /// <summary>
    /// The UTC date and time the job post was created.
    /// This value is automatically set by the backend and cannot be modified externally.
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property to the worker who owns this job post.
    /// </summary>
    public WorkerProfile? Worker { get; set; }
}
