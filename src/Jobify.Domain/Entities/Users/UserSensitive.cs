using System.Net.Mail;
using System.Text.Json.Serialization;
using Jobify.Domain.Components.Security;

namespace Jobify.Domain.Entities.Users;

public class UserSensitive : ISensitiveData
{
    [JsonConstructor]
    private UserSensitive(string firstName, string? middleName, string lastName, string email)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Email = email;
    }

    public string FirstName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public static UserSensitive Create(string firstName, string? middleName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Email format is invalid", nameof(email));

        return new UserSensitive(
            firstName.Trim(),
            middleName?.Trim(),
            lastName.Trim(),
            email.Trim().ToLowerInvariant()
        );
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            MailAddress emailAddress = new(email);
            return emailAddress.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
