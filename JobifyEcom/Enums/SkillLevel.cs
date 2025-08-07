using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Enums;

/// <summary>
/// Represents the proficiency level of a worker's skill.
/// </summary>
public enum SkillLevel
{
	/// <summary>
	/// Entry-level understanding with minimal experience.
	/// </summary>
	[Display(Name = "Beginner")]
	Beginner,

	/// <summary>
	/// Moderate experience and capable of handling basic tasks.
	/// </summary>
	[Display(Name = "Intermediate")]
	Intermediate,

	/// <summary>
	/// Solid experience with the ability to handle complex tasks independently.
	/// </summary>
	[Display(Name = "Advanced")]
	Advanced,

	/// <summary>
	/// Deep expertise with the ability to lead and mentor others.
	/// </summary>
	[Display(Name = "Expert")]
	Expert,
}
