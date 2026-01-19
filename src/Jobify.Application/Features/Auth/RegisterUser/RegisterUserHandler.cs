using Jobify.Application.Constants.Responses;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Enums;
using Jobify.Application.Extensions.Responses;
using Jobify.Application.Helpers;
using Jobify.Application.Models;
using Jobify.Application.Services;
using Jobify.Domain.Entities.Users;
using Jobify.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Application.Features.Auth.RegisterUser;

public class RegisterUserCommandHandler(AppDbContext db, IHashingService hashingService, IDataEncryptionService encryptionService) : IHandler<RegisterUserCommand, OperationResult<Guid>>
{
    public async Task<OperationResult<Guid>> Handle(RegisterUserCommand message, CancellationToken cancellationToken = default)
    {
        string emailHash = hashingService.HashEmail(message.Email);

        if (await db.Users.AnyAsync(u => u.EmailHash == emailHash, cancellationToken))
            throw ResponseCatalog.Auth.EmailAlreadyExists.ToException();

        UserSensitive sensitiveData = UserSensitive.Create(
            firstName: message.FirstName,
            middleName: message.MiddleName,
            lastName: message.LastName,
            email: message.Email
        );

        byte[] sensitiveDataBytes = ObjectByteConverter.SerializeToBytes(sensitiveData);
        byte[] encryptedData = encryptionService.Encrypt(sensitiveDataBytes, CryptoPurpose.UserSensitiveData);

        string passwordHash = await hashingService.HashPasswordAsync(message.Password);

        User user = new(
            userName: message.UserName.Trim(),
            emailHash: emailHash,
            passwordHash: passwordHash
        );

        user.SetEncryptedData(encryptedData);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return ResponseCatalog.Auth.RegistrationSuccessful
            .As<Guid>()
            .WithData(user.Id)
            .ToOperationResult();
    }
}
