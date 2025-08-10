using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.DTOs.Auth;

/// <summary>
/// Represents the data required to register a new user.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Gets or sets the full name of the user.
    /// </summary>
    [Required(ErrorMessage = "Please enter your full name.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password chosen by the user.
    /// </summary>
    [Required(ErrorMessage = "Please enter a password.")]
    [MinLength(6, ErrorMessage = "Your password must be at least 6 characters long.")]
    public string Password { get; set; } = string.Empty;
}
