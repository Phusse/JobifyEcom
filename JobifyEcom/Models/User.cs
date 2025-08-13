using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a registered user of the platform, including login credentials, role, and email verification status.
/// </summary>
public class User
{
    /// <summary>
    /// The unique identifier for the user.
    /// <br>This value is automatically set by the backend and cannot be modified externally.</br>
    /// </summary>
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// The full name of the user.
    /// </summary>
    [Required]
    [MinLength(2)]
    [StringLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// The user's email address. Must be unique and lowercase.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; }

    /// <summary>
    /// The hashed password for authentication.
    /// </summary>
    [Required]
    public required string PasswordHash { get; set; }

    /// <summary>
    /// The role assigned to the user, determining access permissions (e.g., Admin, Customer, Worker).
    /// <br>Defaults to <see cref="UserRole.Customer"/>.</br>
    /// </summary>
    [Required]
    public required UserRole Role { get; set; } = UserRole.Customer;

    /// <summary>
    /// Indicates whether the user's email address has been confirmed.
    /// </summary>
    public bool IsEmailConfirmed { get; set; } = false;

    /// <summary>
    /// A unique token used for verifying the user's email address.
    /// </summary>
    public Guid? EmailConfirmationToken { get; set; }

    /// <summary>
    /// Indicates whether the account is locked and the user cannot log in.
    /// </summary>
    public bool IsLocked { get; set; } = false;

    /// <summary>
    /// The UTC date and time when the account was locked, if applicable.
    /// </summary>
    public DateTime? LockedAt { get; set; }

    /// <summary>
    /// A unique identifier that changes whenever the user's security credentials are updated or tokens are invalidated.
    /// Used to validate JWT tokens and ensure tokens issued before this value are rejected.
    /// </summary>
    [Required]
    public Guid SecurityStamp { get; set; } = Guid.Empty;

    /// <summary>
    /// The UTC date and time when the user was created.
    /// <br>This value is automatically set by the backend and cannot be modified externally.</br>
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// The UTC date and time when the user was last updated.
    /// </summary>
    [Required]
    public required DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The worker profile associated with this user, if the user is a worker.
    /// </summary>
    public WorkerProfile? WorkerProfile { get; set; }
}
