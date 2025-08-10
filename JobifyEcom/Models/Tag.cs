using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a reusable tag or category that can be linked to various entities
/// such as skills, job posts, and potentially other models.
/// </summary>
public class Tag
{
	/// <summary>
	/// The unique identifier for the tag.
	/// This value is automatically set by the backend and cannot be modified externally.
	/// </summary>
	[Key]
	public Guid Id { get; private set; } = Guid.NewGuid();

	/// <summary>
	/// The unique name of the tag (e.g., "Docker", "C#", "Design").
	/// </summary>
	[Required]
	[MinLength(1)]
	[StringLength(100)]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A collection of entity-tag mappings that associate this tag with specific entities.
	/// This enables polymorphic tagging across different types of models.
	/// </summary>
	public ICollection<EntityTag> EntityTags { get; set; } = [];
}
