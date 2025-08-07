using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public UserRole Role { get; set; } = UserRole.Customer;
    public bool IsEmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }
}
