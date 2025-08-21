using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a skill associated with a <see cref="Models.Worker"/>.
/// </summary>
public class Skill
{
	/// <summary>
	/// The unique identifier for the skill.
	/// <para>Automatically generated and cannot be modified externally.</para>
	/// </summary>
	[Key]
	public Guid Id { get; private set; } = Guid.NewGuid();

	/// <summary>
	/// The name of the skill (e.g., "C#", "Docker").
	/// </summary>
	[Required, MinLength(1), StringLength(100)]
	public required string Name { get; set; } = string.Empty;

	/// <summary>
	/// Optional description providing details about the skill.
	/// </summary>
	[StringLength(500)]
	public string? Description { get; set; }

	/// <summary>
	/// The proficiency level of the skill.
	/// </summary>
	[Required]
	public required SkillLevel Level { get; set; } = SkillLevel.Beginner;

	/// <summary>
	/// Optional URL pointing to a certification or proof of skill.
	/// </summary>
	[Url]
	public string? CertificationLink { get; set; }

	/// <summary>
	/// The number of years of experience the worker has with this skill.
	/// </summary>
	[Required, Range(0, 50)]
	public required int YearsOfExperience { get; set; }

	/// <summary>
	/// The ID of the worker this skill belongs to.
	/// </summary>
	[Required]
	public Guid WorkerId { get; set; }

	/// <summary>
	/// Navigation property to the <see cref="Models.Worker"/> who owns this skill.
	/// </summary>
	public Worker? Worker { get; set; }
}
