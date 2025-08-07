using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a link between a <see cref="Models.Tag"/> and a specific entity (e.g., JobPost, Skill).
/// Enables polymorphic tagging by storing the entity type and ID.
/// </summary>
public class EntityTag
{
	/// <summary>
	/// The unique identifier for this entity-tag association.
	/// </summary>
	[Key]
	public Guid Id { get; set; }

	/// <summary>
	/// The foreign key of the associated tag.
	/// </summary>
	[Required]
	public Guid TagId { get; set; }

	/// <summary>
	/// The unique identifier of the entity that is tagged (e.g., JobPost ID, Skill ID).
	/// </summary>
	[Required]
	public Guid EntityId { get; set; }

	/// <summary>
	/// The type of entity that this tag is associated with (e.g., JobPost, Skill).
	/// </summary>
	[Required]
	public EntityType EntityType { get; set; }

	/// <summary>
	/// Navigation property to the associated <see cref="Models.Tag"/>.
	/// </summary>
	public Tag? Tag { get; set; }
}
