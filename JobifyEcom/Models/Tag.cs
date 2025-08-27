using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a reusable tag or category that can be linked to various entities
/// such as skills, jobs, and potentially other models.
/// </summary>
public class Tag
{
	/// <summary>
	/// The unique identifier for this tag.
	/// <para>Automatically generated and cannot be modified externally.</para>
	/// </summary>
	[Key]
	public Guid Id { get; private set; } = Guid.NewGuid();

	/// <summary>
	/// The unique name of the tag (e.g., "Docker", "C#", "Design").
	/// </summary>
	[Required]
	[MinLength(1)]
	[StringLength(100)]
	public required string Name { get; set; } = string.Empty;

	/// <summary>
	/// Collection of entity-tag mappings linking this tag to various entities.
	/// Enables polymorphic tagging across different entity types <see cref="Enums.EntityType"/>.
	/// </summary>
	public ICollection<EntityTag> TaggedEntities { get; set; } = [];
}
