using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.DTOs.Auth;

/// <summary>
/// Represents the data required to log in a user.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the user's email address used for login.
    /// </summary>
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's password.
    /// </summary>
    [Required(ErrorMessage = "Please enter your password.")]
    [MinLength(6, ErrorMessage = "Your password must be at least 6 characters long.")]
    public string Password { get; set; } = string.Empty;
}
