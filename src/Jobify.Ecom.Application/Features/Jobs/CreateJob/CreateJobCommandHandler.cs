using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Persistence.Context;

namespace Jobify.Ecom.Application.Features.Jobs.CreateJob;

internal sealed class CreateJobCommandHandler(AppDbContext context)
    : IHandler<CreateJobCommand, OperationResult<Guid>>
{
    public async Task<OperationResult<Guid>> Handle(CreateJobCommand message, CancellationToken cancellationToken)
    {
        if (message.PostedByUserId is null)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        Job job = new(
            message.PostedByUserId.Value,
            message.Title,
            message.Description,
            message.JobType,
            message.MinSalary,
            message.MaxSalary,
            message.ClosingDate
        );

        context.Jobs.Add(job);
        await context.SaveChangesAsync(cancellationToken);

        return ResponseCatalog.Job.JobCreated
            .As<Guid>()
            .WithData(job.Id)
            .ToOperationResult();
    }
}
