using Jobify.Application.Constants.Responses;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Enums;
using Jobify.Application.Extensions.Entities;
using Jobify.Application.Extensions.Responses;
using Jobify.Application.Features.Users.Models;
using Jobify.Application.Helpers;
using Jobify.Application.Models;
using Jobify.Application.Services;
using Jobify.Domain.Entities.Users;
using Jobify.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Application.Features.Users.GetCurrentUser;

public class GetCurrentUserHandler(AppDbContext db, IDataEncryptionService encryptionService)
    : IHandler<GetCurrentUserRequest, OperationResult<UserResponse>>
{
    public async Task<OperationResult<UserResponse>> Handle(GetCurrentUserRequest message, CancellationToken cancellationToken = default)
    {
        if (message.UserId is null)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        User user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == message.UserId.Value, cancellationToken)
            ?? throw ResponseCatalog.Auth.InvalidSession.ToException();

        if (user.IsLocked)
            throw ResponseCatalog.Auth.AccountLocked.ToException();

        UserSensitive sensitiveData = ObjectByteConverter.DeserializeFromBytes<UserSensitive>(
            encryptionService.Decrypt(user.EncryptedData, CryptoPurpose.UserSensitiveData)
        );

        user.SetSensitiveData(sensitiveData);

        return ResponseCatalog.User.UserRetrieved
            .As<UserResponse>()
            .WithData(user.ToUserResponse())
            .ToOperationResult();
    }
}
