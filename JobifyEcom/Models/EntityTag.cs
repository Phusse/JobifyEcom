using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a link between a <see cref="Models.Tag"/> and a specific entity <see cref="Enums.EntityType"/>.
/// Supports polymorphic tagging by storing the entity's type and identifier.
/// </summary>
public class EntityTag
{
	/// <summary>
	/// The unique identifier for this entity-tag association.
	/// <para>Automatically generated and cannot be modified externally.</para>
	/// </summary>
	[Key]
	public Guid Id { get; private set; } = Guid.NewGuid();

	/// <summary>
	/// The foreign key referencing the associated <see cref="Models.Tag"/>.
	/// </summary>
	[Required]
	public required Guid TagId { get; set; }

	/// <summary>
	/// The unique identifier of the entity that is tagged (e.g., JobPost ID, Skill ID).
	/// </summary>
	[Required]
	public required Guid EntityId { get; set; }

	/// <summary>
	/// The type of entity this tag is associated with <see cref="Enums.EntityType"/>.
	/// </summary>
	[Required]
	public required EntityType EntityType { get; set; }

	/// <summary>
	/// Navigation property to the associated <see cref="Models.Tag"/>.
	/// </summary>
	public Tag? Tag { get; set; }
}
