using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Enums;

/// <summary>
/// Defines user roles for authorization and access control across the platform.
/// Enum values are code-friendly; Display(Name) values are UX-friendly for frontend.
/// </summary>
public enum SystemRole
{
    /// <summary>
    /// Baseline platform user with standard capabilities (browse/request jobs).
    /// </summary>
    [Display(Name = "Member")]
    User,

    /// <summary>
    /// User with extra capabilities: can apply to jobs and showcase portfolio.
    /// </summary>
    [Display(Name = "Freelancer")]
    Worker,

    /// <summary>
    /// Moderates users and content; not a regular platform participant.
    /// </summary>
    [Display(Name = "Moderator")]
    Admin,

    /// <summary>
    /// Full platform owner; can assign or remove admin privileges.
    /// </summary>
    [Display(Name = "Owner")]
    SuperAdmin,
}
