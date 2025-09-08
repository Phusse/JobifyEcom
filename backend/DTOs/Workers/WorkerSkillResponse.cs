using JobifyEcom.Enums;

namespace JobifyEcom.DTOs.Workers;

/// <summary>
/// Represents a skill belonging to a worker, including details,
/// tags, and its verification status.
/// </summary>
public class WorkerSkillResponse
{
	/// <summary>
	/// The unique identifier of the skill.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// The identifier of the worker who owns this skill.
	/// </summary>
	public required Guid WorkerId { get; set; }

	/// <summary>
	/// The name of the skill (e.g., "C#", "Docker").
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Optional description providing more details about the skill.
	/// </summary>
	public required string? Description { get; set; }

	/// <summary>
	/// The proficiency level of the skill (Beginner, Intermediate, Advanced, Expert).
	/// </summary>
	public required SkillLevel Level { get; set; }

	/// <summary>
	/// The number of years of experience the worker has with this skill.
	/// </summary>
	public required int YearsOfExperience { get; set; }

	/// <summary>
	/// Optional link to certification or proof of this skill.
	/// </summary>
	public required string? CertificationLink { get; set; }

	/// <summary>
	/// Tags that categorize this skill (e.g., "Frontend", "React").
	/// Useful for searching and filtering workers by skill.
	/// </summary>
	public required IReadOnlyList<string> Tags { get; set; } = [];

	/// <summary>
	/// The verification status of the skill.
	/// Only verified skills are considered in public search results.
	/// </summary>
	public required VerificationStatus VerificationStatus { get; set; }
}
