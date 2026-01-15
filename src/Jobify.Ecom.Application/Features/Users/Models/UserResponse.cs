using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Application.Features.Users.Models;

public record UserResponse(
    Guid Id,
    string UserName,
    string FirstName,
    string MiddleName,
    string LastName,
    SystemRole Role
);
