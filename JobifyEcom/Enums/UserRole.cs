using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Enums;

/// <summary>
/// Enum used to define user roles, granting them authorization and access to specific endpoints.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// A user who can browse and purchase products or services.
    /// </summary>
    [Display(Name = "Customer")]
    Customer,

    /// <summary>
    /// A user who performs services or fulfills jobs on the platform.
    /// </summary>
    [Display(Name = "Worker")]
    Worker,

    /// <summary>
    /// A user with full access to manage the platform, users, and content.
    /// </summary>
    [Display(Name = "Administrator")]
    Admin,
}
