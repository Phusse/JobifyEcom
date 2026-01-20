using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.JobApplications;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.JobApplications.ApplyForJob;

public class ApplyForJobCommandHandler(AppDbContext context) : IHandler<ApplyForJobCommand, OperationResult<Guid>>
{
    public async Task<OperationResult<Guid>> Handle(ApplyForJobCommand message, CancellationToken cancellationToken = default)
    {
        if (message.ApplicantUserId is not Guid applicantUserId)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        Job job = await context.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == message.JobId, cancellationToken)
            ?? throw ResponseCatalog.Job.NotFound.ToException();

        if (job.PostedByUserId == applicantUserId)
            throw ResponseCatalog.JobApplication.CannotApplyOwnJob.ToException();

        if (job.ClosingDate <= DateTime.UtcNow)
            throw ResponseCatalog.JobApplication.JobClosed.ToException();

        bool alreadyApplied = await context.JobApplications
            .AnyAsync(ja => ja.JobId == message.JobId && ja.ApplicantUserId == applicantUserId, cancellationToken);

        if (alreadyApplied)
            throw ResponseCatalog.JobApplication.AlreadyApplied.ToException();

        JobApplication application = new(message.JobId, applicantUserId);

        context.JobApplications.Add(application);
        await context.SaveChangesAsync(cancellationToken);

        return ResponseCatalog.JobApplication.Applied
            .As<Guid>()
            .WithData(application.Id)
            .ToOperationResult();
    }
}
