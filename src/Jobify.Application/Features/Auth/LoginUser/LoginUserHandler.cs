using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Constants.Responses;
using Jobify.Application.Extensions.Responses;
using Jobify.Application.Models;
using Jobify.Application.Services;
using Jobify.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Jobify.Domain.Entities.UserSessions;
using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Extensions.Entities;
using Jobify.Application.Features.Auth.Extensions;

namespace Jobify.Application.Features.Auth.LoginUser;

public class LoginUserHandler(AppDbContext db, IHashingService hashingService, SessionManagementService sessionService)
    : IHandler<LoginUserRequest, OperationResult<SessionTimestampsResponse>>
{
    public async Task<OperationResult<SessionTimestampsResponse>> Handle(LoginUserRequest message, CancellationToken cancellationToken = default)
    {
        string emailHash = hashingService.HashEmail(message.Identifier);

        var userDto = await db.Users
            .AsNoTracking()
            .Where(u => u.EmailHash == emailHash || u.UserName == message.Identifier)
            .Select(u => new
            {
                u.Id,
                u.PasswordHash,
                u.IsLocked,
                u.Role
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw ResponseCatalog.Auth.InvalidCredentials.ToException();

        if (userDto.IsLocked)
            throw ResponseCatalog.Auth.InvalidCredentials.ToException();

        bool isPasswordValid = await hashingService.VerifyPasswordAsync(message.Password, userDto.PasswordHash);

        if (!isPasswordValid)
            throw ResponseCatalog.Auth.InvalidCredentials.ToException();

        UserSession sessionData = await sessionService.CreateSessionAsync(userDto.Id, userDto.Role, message.RememberMe, cancellationToken);

        SessionTimestampsResponse data = sessionData.ToTimestampsResponse();

        return ResponseCatalog.Auth.LoginSuccessful
            .As<SessionTimestampsResponse>()
            .WithData(data)
            .ToOperationResult();
    }
}
