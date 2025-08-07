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
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The full name of the user.
    /// </summary>
    [Required]
    [MinLength(2)]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The user's email address. Must be unique and lowercase.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The hashed password for authentication.
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// The role assigned to the user (e.g., Admin, Customer, Worker).
    /// </summary>
    [Required]
    public UserRole Role { get; set; } = UserRole.Customer;

    /// <summary>
    /// Indicates whether the user's email address has been confirmed.
    /// </summary>
    public bool IsEmailConfirmed { get; set; } = false;

    /// <summary>
    /// A unique token used for verifying the user's email address.
    /// </summary>
    public Guid? EmailConfirmationToken { get; set; }

    /// <summary>
    /// The worker profile associated with this user, if the user is a worker.
    /// </summary>
    public WorkerProfile? WorkerProfile { get; set; }
}
