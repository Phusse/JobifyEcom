using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.JobApplications;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.JobApplications.WithdrawApplication;

public class WithdrawApplicationCommandHandler(AppDbContext context) : IHandler<WithdrawApplicationCommand, OperationResult<object>>
{
    public async Task<OperationResult<object>> Handle(WithdrawApplicationCommand message, CancellationToken cancellationToken = default)
    {
        if (message.ApplicantUserId is not Guid applicantUserId)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        JobApplication application = await context.JobApplications
            .FirstOrDefaultAsync(ja => ja.Id == message.ApplicationId, cancellationToken)
            ?? throw ResponseCatalog.JobApplication.NotFound.ToException();

        if (application.ApplicantUserId != applicantUserId)
            throw ResponseCatalog.JobApplication.ModificationForbidden.ToException();

        context.JobApplications.Remove(application);
        await context.SaveChangesAsync(cancellationToken);

        return ResponseCatalog.JobApplication.Withdrawn
            .As<object>()
            .ToOperationResult();
    }
}
