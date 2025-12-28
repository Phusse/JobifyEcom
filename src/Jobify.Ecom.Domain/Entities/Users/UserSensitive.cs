using System.Net.Mail;
using Jobify.Ecom.Domain.Components.Security;

namespace Jobify.Ecom.Domain.Entities.Users;

public class UserSensitive : ISensitiveData
{
    public string FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }

    private UserSensitive(string firstName, string? middleName, string lastName, string email)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Email = email;
    }

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
            firstName.Trim().ToLowerInvariant(),
            middleName?.Trim().ToLowerInvariant(),
            lastName.Trim().ToLowerInvariant(),
            email.Trim().ToLowerInvariant()
        );
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            MailAddress emailAdress = new(email);
            return emailAdress.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
