using System.ComponentModel.DataAnnotations;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public string Role { get; set; } = "Customer";
    public bool IsEmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }

}
