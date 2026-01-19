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

public class GetUserByIdQueryHandler(AppDbContext db, IDataEncryptionService encryptionService) : IHandler<GetUserByIdQuery, OperationResult<UserResponse>>
{
    public async Task<OperationResult<UserResponse>> Handle(GetUserByIdQuery message, CancellationToken cancellationToken = default)
    {
        User? user = await db.Users
            .FirstOrDefaultAsync(u => u.Id == message.Id, cancellationToken)
            ?? throw ResponseCatalog.User.NotFound.ToException();

        UserSensitive sensitiveData = ObjectByteConverter.DeserializeFromBytes<UserSensitive>(
            encryptionService.Decrypt(user.EncryptedData, CryptoPurpose.UserSensitiveData)
        );

        user.SetSensitiveData(sensitiveData);

        return ResponseCatalog.User.Retrieved
            .As<UserResponse>()
            .WithData(user.ToUserResponse())
            .ToOperationResult();
    }
}
