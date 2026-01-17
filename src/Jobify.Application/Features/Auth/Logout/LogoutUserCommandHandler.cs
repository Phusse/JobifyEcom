using Jobify.Application.Constants.Responses;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Extensions.Responses;
using Jobify.Application.Models;
using Jobify.Application.Services;

namespace Jobify.Application.Features.Auth.Logout;

public class LogoutUserCommandHandler(SessionManagementService sessionService) : IHandler<LogoutUserCommand, OperationResult<object>>
{
    public async Task<OperationResult<object>> Handle(LogoutUserCommand message, CancellationToken cancellationToken = default)
    {
        if (message.SessionId.HasValue)
            await sessionService.RevokeSessionAsync(message.SessionId.Value, cancellationToken);

        return ResponseCatalog.Auth.LogoutSuccessful
            .As<object>()
            .ToOperationResult();
    }
}
