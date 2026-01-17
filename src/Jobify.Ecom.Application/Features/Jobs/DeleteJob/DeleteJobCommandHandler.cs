using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.Jobs.DeleteJob;

internal sealed class DeleteJobCommandHandler(AppDbContext context) : IHandler<DeleteJobCommand, OperationResult<object>>
{
    public async Task<OperationResult<object>> Handle(DeleteJobCommand message, CancellationToken cancellationToken)
    {
        if (message.DeletedByUserId is null)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        Job job = await context.Jobs
            .FirstOrDefaultAsync(j => j.Id == message.JobId, cancellationToken)
            ?? throw ResponseCatalog.Job.JobNotFound.ToException();

        if (job.PostedByUserId != message.DeletedByUserId.Value)
            throw ResponseCatalog.Job.JobModificationForbidden.ToException();

        context.Jobs.Remove(job);
        await context.SaveChangesAsync(cancellationToken);

        return ResponseCatalog.Job.JobDeleted
            .As<object>()
            .ToOperationResult();
    }
}
