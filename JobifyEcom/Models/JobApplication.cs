using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents an application submitted by a <see cref="Worker"/> for a specific <see cref="Models.Job"/>.
/// Tracks the status and submission date.
/// </summary>
public class JobApplication
{
    /// <summary>
    /// The unique identifier for this job application.
	/// <para>Automatically generated and cannot be modified externally.</para>
    /// </summary>
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the worker submitting this application.
    /// </summary>
    [Required]
    public required Guid WorkerId { get; set; }

    /// <summary>
    /// Navigation property to the <see cref="Worker"/> submitting the application.
    /// </summary>
    public Worker Applicant { get; set; } = null!;

    /// <summary>
    /// The ID of the job post being applied to.
    /// </summary>
    [Required]
    public required Guid JobPostId { get; set; }

    /// <summary>
    /// Navigation property to the <see cref="Models.Job"/> this application is for.
    /// </summary>
    public Job Job { get; set; } = null!;

    /// <summary>
    /// The current status of the application (e.g., Pending, Accepted, Rejected).
    /// </summary>
    [Required]
    public required JobApplicationStatus Status { get; set; } = JobApplicationStatus.Pending;

    /// <summary>
    /// The UTC datetime when this application was submitted.
	/// <para>Automatically generated and cannot be modified externally.</para>
    /// </summary>
    public DateTime DateRequested { get; private set; } = DateTime.UtcNow;
}
