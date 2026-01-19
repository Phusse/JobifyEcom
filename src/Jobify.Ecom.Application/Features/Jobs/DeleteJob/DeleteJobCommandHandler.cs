using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.Jobs.DeleteJob;

public class DeleteJobCommandHandler(AppDbContext context) : IHandler<DeleteJobCommand, OperationResult<object>>
{
    public async Task<OperationResult<object>> Handle(DeleteJobCommand message, CancellationToken cancellationToken = default)
    {
        if (message.DeletedByUserId is not Guid userId)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        Job job = await context.Jobs
            .FirstOrDefaultAsync(j => j.Id == message.JobId, cancellationToken)
            ?? throw ResponseCatalog.Job.NotFound.ToException();

        if (job.PostedByUserId != userId)
            throw ResponseCatalog.Job.ModificationForbidden.ToException();

        context.Jobs.Remove(job);
        await context.SaveChangesAsync(cancellationToken);

        return ResponseCatalog.Job.Deleted
            .As<object>()
            .ToOperationResult();
    }
}
