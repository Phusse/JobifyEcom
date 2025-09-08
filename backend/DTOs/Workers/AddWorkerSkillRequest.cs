using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.DTOs.Workers;

/// <summary>
/// Represents the data required to add a new skill to a worker's profile.
/// </summary>
public class AddWorkerSkillRequest
{
	/// <summary>
	/// The name of the skill (e.g., "C#", "Docker").
	/// </summary>
	[Required(ErrorMessage = "Skill name is required.")]
	[MinLength(2, ErrorMessage = "Skill name must be at least 2 characters long.")]
	[MaxLength(100, ErrorMessage = "Skill name cannot exceed 100 characters.")]
	public required string Name { get; set; }

	/// <summary>
	/// Optional description providing additional details about the skill.
	/// </summary>
	[MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
	public string? Description { get; set; }

	/// <summary>
	/// The proficiency level of the skill (e.g., Beginner, Intermediate, Advanced, Expert).
	/// </summary>
	[Required(ErrorMessage = "Skill level is required.")]
	public required SkillLevel Level { get; set; }

	/// <summary>
	/// The number of years of experience with this skill.
	/// </summary>
	[Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50.")]
	public required int YearsOfExperience { get; set; }

	/// <summary>
	/// Optional URL pointing to a certification or proof of skill.
	/// </summary>
	[Url(ErrorMessage = "Certification link must be a valid URL.")]
	public string? CertificationLink { get; set; }

	/// <summary>
	/// A list of tags associated with this skill (e.g., "Frontend", "React").
	/// At least one tag is required for search and categorization.
	/// </summary>
	[MinLength(1, ErrorMessage = "At least one tag is required.")]
	public required List<string> Tags { get; set; } = [];
}
