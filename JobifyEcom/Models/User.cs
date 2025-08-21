using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents a user in the system who can post jobs, submit ratings, and optionally have a worker profile.
/// </summary>
public class User
{
    /// <summary>
    /// The unique identifier for this user.
    /// <para>Automatically generated and cannot be modified externally.</para>
    /// </summary>
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// The full name of the user.
    /// </summary>
    [Required, MinLength(2), StringLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// The user's email address.
    /// Must be unique (enforced in DB context).
    /// </summary>
    [Required, EmailAddress, StringLength(100)]
    public required string Email { get; set; }

    /// <summary>
    /// The hashed password for authentication.
    /// </summary>
    [Required]
    public required string PasswordHash { get; set; }

    /// <summary>
    /// Optional token used for password reset.
    /// </summary>
    public Guid? PasswordResetToken { get; set; }

    /// <summary>
    /// Optional expiry date for the password reset token.
    /// </summary>
    public DateTime? PasswordResetTokenExpiry { get; set; }

    /// <summary>
    /// A short biography or description for the user.
    /// </summary>
    [StringLength(250)]
    public string? Bio { get; set; }

    /// <summary>
    /// Backing field for the <see cref="StaffRole"/> property.
    /// </summary>
    private SystemRole? _staffRole;

    /// <summary>
    /// Optional staff role of the user (<see cref="SystemRole.Admin"/> or <see cref="SystemRole.SuperAdmin"/>). <c>Null</c> if regular user.
    /// </summary>
    public SystemRole? StaffRole
    {
        get => _staffRole;
        set
        {
            if (value is not (null or SystemRole.Admin or SystemRole.SuperAdmin))
            {
                throw new ArgumentException($"Staff Role can only be {SystemRole.Admin}, {SystemRole.SuperAdmin}, or null.");
            }

            _staffRole = value;
        }
    }

    /// <summary>
    /// Indicates whether the user's email has been confirmed.
    /// </summary>
    public bool IsEmailConfirmed { get; set; } = false;

    /// <summary>
    /// Optional token used for email confirmation.
    /// </summary>
    public Guid? EmailConfirmationToken { get; set; }

    /// <summary>
    /// Indicates whether the user account is locked.
    /// </summary>
    public bool IsLocked { get; set; } = false;

    /// <summary>
    /// The UTC datetime when the account was locked.
    /// </summary>
    public DateTime? LockedAt { get; set; }

    /// <summary>
    /// Security stamp used for authentication validation.
    /// </summary>
    [Required]
    public Guid SecurityStamp { get; set; } = Guid.Empty;

    /// <summary>
    /// UTC datetime when the user was created.
    /// <para>Automatically generated and cannot be modified externally.</para>
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC datetime when the user was last updated.
    /// </summary>
    [Required]
    public required DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets a value indicating whether the user has a worker profile.
    /// <c>True</c> if the user has an associated <see cref="WorkerProfile"/>, otherwise <c>False</c>.
    /// <para>Note: Ensure that the <see cref="WorkerProfile"/> entity is included when querying the user;
    /// otherwise, this property will always return <c>False</c>.</para>
    /// </summary>
    public bool IsWorker => WorkerProfile is not null;

    /// <summary>
    /// Optional associated worker profile if the user is also a worker.
    /// </summary>
    public Worker? WorkerProfile { get; set; }

    /// <summary>
    /// Jobs posted by this user.
    /// </summary>
    public ICollection<JobPost> JobsPosted { get; set; } = [];

    /// <summary>
    /// Ratings submitted by this user.
    /// </summary>
    public ICollection<Rating> RatingsSubmitted { get; set; } = [];
}
