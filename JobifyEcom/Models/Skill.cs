using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a specific skill that a worker possesses,
/// including proficiency level, years of experience, and optional certification.
/// </summary>
public class Skill
{
	/// <summary>
	/// The unique identifier for the skill entry.
	/// This value is automatically set by the backend and cannot be modified externally.
	/// </summary>
	[Key]
	public Guid Id { get; private set; } = Guid.NewGuid();

	/// <summary>
	/// The name of the skill (e.g., "C#", "Photoshop").
	/// </summary>
	[Required]
	[MinLength(1)]
	[StringLength(100)]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A brief description of the skill (optional).
	/// </summary>
	[StringLength(500)]
	public string? Description { get; set; }

	/// <summary>
	/// The skill proficiency level (e.g., Beginner, Intermediate, Advanced, Expert).
	/// </summary>
	[Required]
	public SkillLevel Level { get; set; } = SkillLevel.Beginner;

	/// <summary>
	/// An optional URL linking to a certification or portfolio item that verifies the skill.
	/// </summary>
	[Url]
	public string? CertificationLink { get; set; }

	/// <summary>
	/// The number of years of experience the worker has with this skill.
	/// </summary>
	[Required]
	[Range(0, 50)]
	public int YearsOfExperience { get; set; }

	/// <summary>
	/// The foreign key that links this skill to the associated worker profile.
	/// </summary>
	[Required]
	public Guid WorkerProfileId { get; set; }

	/// <summary>
	/// The worker profile that owns this skill.
	/// </summary>
	public WorkerProfile? WorkerProfile { get; set; }
}
