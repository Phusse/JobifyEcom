using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Auth.RegisterUser;

public record RegisterUserCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    string UserName,
    string Email,
    string Password
) : IMessage<OperationResult<Guid>>;
