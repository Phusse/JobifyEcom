using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Enums;
using Jobify.Ecom.Application.Extensions.Entities;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Features.Users.Models;
using Jobify.Ecom.Application.Helpers;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Application.Services;
using Jobify.Ecom.Domain.Entities.Users;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.Users.GetCurrentUser;

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
