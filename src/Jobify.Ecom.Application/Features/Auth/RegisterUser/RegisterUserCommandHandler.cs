using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Users;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.Auth.RegisterUser;

public class RegisterUserCommandHandler(AppDbContext db)
    : IHandler<RegisterUserCommand, OperationResult<Guid>>
{
    public async Task<OperationResult<Guid>> Handle(RegisterUserCommand message, CancellationToken cancellationToken = default)
    {
        if (message.SourceUserId is null)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        if (await db.Users.AnyAsync(u => u.SourceUserId == message.SourceUserId, cancellationToken))
            throw ResponseCatalog.Auth.UserAlreadyExists.ToException();

        User user = new(message.SourceUserId.Value);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return ResponseCatalog.Auth.RegistrationSuccessful
            .As<Guid>()
            .WithData(user.Id)
            .ToOperationResult();
    }
}
