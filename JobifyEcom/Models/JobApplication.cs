using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a customer's application for a specific job post.
/// </summary>
public class JobApplication
{
    /// <summary>
    /// The unique identifier for the job application.
    /// <br>This value is automatically set by the backend and cannot be modified externally.</br>
    /// </summary>
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the customer (user) who submitted the application.
    /// </summary>
    [Required]
    public required Guid CustomerId { get; set; }

    /// <summary>
    /// The ID of the job post to which the application was submitted.
    /// </summary>
    [Required]
    public required Guid JobPostId { get; set; }

    /// <summary>
    /// The current status of the job application (e.g., Pending, Accepted, Rejected).
    /// </summary>
    [Required]
    public required JobApplicationStatus Status { get; set; } = JobApplicationStatus.Pending;

    /// <summary>
    /// The UTC date and time when the application was submitted.
    /// <br>This value is automatically set by the backend and cannot be modified externally.</br>
    /// </summary>
    public DateTime DateRequested { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// The customer who submitted the application.
    /// </summary>
    public User? Customer { get; set; }

    /// <summary>
    /// The job post that the application is associated with.
    /// </summary>
    public JobPost? JobPost { get; set; }
}
