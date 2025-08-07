using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a reusable tag or category used across skills and job posts.
/// </summary>
public class Tag
{
	/// <summary>
	/// The unique identifier for the tag.
	/// </summary>
	[Key]
	public Guid Id { get; set; }

	/// <summary>
	/// The unique name of the tag.
	/// </summary>
	[Required]
	[StringLength(100)]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// The job posts associated with this tag.
	/// </summary>
	public ICollection<JobPost> JobPosts { get; set; } = [];

	/// <summary>
	/// The skills associated with this tag.
	/// </summary>
	public ICollection<Skill> Skills { get; set; } = [];
}
