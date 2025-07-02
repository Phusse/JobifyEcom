using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [RegularExpression(@"^[\w\.-]+@[\w\.-]+\.\w{2,}$", ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    public string Password { get; set; }
}