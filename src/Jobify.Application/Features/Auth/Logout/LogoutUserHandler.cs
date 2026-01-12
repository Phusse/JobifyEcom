using Jobify.Application.Constants.Responses;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Extensions.Responses;
using Jobify.Application.Models;
using Jobify.Application.Services;

namespace Jobify.Application.Features.Auth.Logout;

public class LogoutUserHandler(SessionManagementService sessionService)
    : IHandler<LogoutUserRequest, OperationResult<object>>
{
    public async Task<OperationResult<object>> Handle(LogoutUserRequest message, CancellationToken cancellationToken = default)
    {
        if (message.SessionId.HasValue)
            await sessionService.RevokeSessionAsync(message.SessionId.Value, cancellationToken);

        return ResponseCatalog.Auth.LogoutSuccessful
            .As<object>()
            .ToOperationResult();
    }
}
