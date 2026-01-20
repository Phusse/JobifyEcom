using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Enums;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.JobApplications;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.JobApplications.UpdateApplicationStatus;

public class UpdateApplicationStatusCommandHandler(AppDbContext context) : IHandler<UpdateApplicationStatusCommand, OperationResult<object>>
{
    public async Task<OperationResult<object>> Handle(UpdateApplicationStatusCommand message, CancellationToken cancellationToken = default)
    {
        if (message.RequestingUserId is not Guid requestingUserId)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        JobApplication application = await context.JobApplications
            .Include(ja => ja.Job)
            .FirstOrDefaultAsync(ja => ja.Id == message.ApplicationId, cancellationToken)
            ?? throw ResponseCatalog.JobApplication.NotFound.ToException();

        if (application.Job.PostedByUserId != requestingUserId)
            throw ResponseCatalog.JobApplication.ModificationForbidden.ToException();

        if (!Enum.IsDefined(message.NewStatus))
        {
            throw ResponseCatalog.JobApplication.InvalidStatusTransition
                .WithDetails(new ResponseDetail(
                    Message: $"'{message.NewStatus}' is not a valid job application status.",
                    Severity: ResponseSeverity.Error
                ))
                .ToException();
        }

        if (!JobApplication.IsValidTransition(application.Status, message.NewStatus))
        {
            throw ResponseCatalog.JobApplication.InvalidStatusTransition
                .WithDetails(new ResponseDetail(
                    Message: $"Cannot change status from '{application.Status}' to '{message.NewStatus}'.",
                    Severity: ResponseSeverity.Error
                ))
                .ToException();
        }

        application.UpdateStatus(message.NewStatus);
        await context.SaveChangesAsync(cancellationToken);

        return ResponseCatalog.JobApplication.StatusUpdated
            .As<object>()
            .ToOperationResult();
    }
}
