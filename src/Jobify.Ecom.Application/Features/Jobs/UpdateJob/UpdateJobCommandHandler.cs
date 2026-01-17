using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.Jobs.UpdateJob;

internal class UpdateJobCommandHandler(AppDbContext context)
    : IHandler<UpdateJobCommand, OperationResult<Guid>>
{
    public async Task<OperationResult<Guid>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        if (request.UpdatedByUserId == null)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        Job? job = await context.Jobs
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken)
            ?? throw ResponseCatalog.Job.JobNotFound.ToException();

        if (job.PostedByUserId != request.UpdatedByUserId.Value)
            throw ResponseCatalog.Job.JobModificationForbidden.ToException();

        job.Update(
            request.Title,
            request.Description,
            request.JobType,
            request.MinSalary,
            request.MaxSalary,
            request.ClosingDate
        );

        await context.SaveChangesAsync(cancellationToken);

        return ResponseCatalog.Job.JobUpdated
            .As<Guid>()
            .WithData(job.Id)
            .ToOperationResult();
    }
}
