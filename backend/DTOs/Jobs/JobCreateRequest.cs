using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.DTOs.Jobs;

/// <summary>
/// Request payload for creating a new Job.
/// Contains the essential details required to post a job.
/// </summary>
public class JobCreateRequest
{
	/// <summary>
	/// The title of the job post.
	/// </summary>
	[Required(ErrorMessage = "Please provide a job title.")]
	[MinLength(5, ErrorMessage = "The job title must be at least 5 characters long.")]
	[StringLength(100, ErrorMessage = "The job title cannot be longer than 100 characters.")]
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// The detailed description of the job post.
	/// </summary>
	[Required(ErrorMessage = "Please provide a job description.")]
	[MinLength(10, ErrorMessage = "The job description must be at least 10 characters long.")]
	[StringLength(2000, ErrorMessage = "The job description cannot exceed 2000 characters.")]
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The price offered for completing the job.
	/// </summary>
	[Required(ErrorMessage = "Please specify a price for the job.")]
	[Range(0.01, 1000000, ErrorMessage = "The job price must be between 0.01 and 1,000,000.")]
	public decimal Price { get; set; }
}
