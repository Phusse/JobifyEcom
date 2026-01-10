using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Auth.RegisterUser;

public record RegisterUserRequest(
    string FirstName,
    string? MiddleName,
    string LastName,
    string UserName,
    string Email,
    string Password
) : IRequest<OperationResult<Guid>>;
