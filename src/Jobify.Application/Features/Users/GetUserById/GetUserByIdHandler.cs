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

namespace Jobify.Application.Features.Users.GetUserById;

public class GetUserByIdHandler(AppDbContext db, IDataEncryptionService encryptionService)
    : IHandler<GetUserByIdRequest, OperationResult<UserResponse>>
{
    public async Task<OperationResult<UserResponse>> Handle(GetUserByIdRequest message, CancellationToken cancellationToken = default)
    {
        User? user = await db.Users
            .FirstOrDefaultAsync(u => u.Id == message.Id, cancellationToken)
            ?? throw ResponseCatalog.User.UserNotFound.ToException();

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
