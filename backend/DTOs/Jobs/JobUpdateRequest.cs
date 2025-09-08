using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.DTOs.Jobs;

/// <summary>
/// Request payload for updating an existing Job.
/// All fields are optional — only non-null values will be updated.
/// </summary>
public class JobUpdateRequest
{
	/// <summary>
	/// The updated title of the job post.
	/// </summary>
	[MinLength(5, ErrorMessage = "Title must be at least 5 characters long.")]
	[StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
	public string? Title { get; set; }

	/// <summary>
	/// The updated detailed description of the job post.
	/// </summary>
	[MinLength(10, ErrorMessage = "Description must be at least 10 characters long.")]
	[StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
	public string? Description { get; set; }

	/// <summary>
	/// The updated price offered for completing the job.
	/// </summary>
	[Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000.")]
	public decimal? Price { get; set; }

	/// <summary>
	/// The updated status of the job (e.g., Open, InProgress, Completed).
	/// </summary>
	public JobStatus? Status { get; set; }
}
