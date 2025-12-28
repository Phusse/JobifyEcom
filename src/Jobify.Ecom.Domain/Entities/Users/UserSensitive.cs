using Jobify.Ecom.Domain.Components.Security;

namespace Jobify.Ecom.Domain.Entities.Users;

public class UserSensitive : ISensitiveData
{
    public required string FirstName { get; set; }
    public required string MiddleName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
}
