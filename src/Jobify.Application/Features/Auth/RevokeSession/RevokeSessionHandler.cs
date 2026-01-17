using Jobify.Application.Constants.Responses;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Extensions.Responses;
using Jobify.Application.Models;
using Jobify.Application.Services;

namespace Jobify.Application.Features.Auth.RevokeSession;

public class RevokeSessionCommandHandler(SessionManagementService sessionService) : IHandler<RevokeSessionCommand, OperationResult<object>>
{
    public async Task<OperationResult<object>> Handle(RevokeSessionCommand request, CancellationToken cancellationToken = default)
    {
        if (request.SessionId.HasValue)
            await sessionService.RevokeSessionAsync(request.SessionId.Value, cancellationToken);

        return ResponseCatalog.Auth.SessionRevoked
            .As<object>()
            .ToOperationResult();
    }
}
