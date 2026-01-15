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

namespace Jobify.Ecom.Application.Features.Users.GetUserById;

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
