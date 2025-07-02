using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

public class RegisterDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }
}
