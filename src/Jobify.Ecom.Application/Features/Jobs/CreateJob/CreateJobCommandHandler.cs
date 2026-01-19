using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Persistence.Context;

namespace Jobify.Ecom.Application.Features.Jobs.CreateJob;

public class CreateJobCommandHandler(AppDbContext context) : IHandler<CreateJobCommand, OperationResult<Guid>>
{
    public async Task<OperationResult<Guid>> Handle(CreateJobCommand message, CancellationToken cancellationToken = default)
    {
        if (message.PostedByUserId is not Guid userId)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        Job job = new(
            userId,
            message.Title,
            message.Description,
            message.JobType,
            message.MinSalary,
            message.MaxSalary,
            message.ClosingDate
        );

        context.Jobs.Add(job);
        await context.SaveChangesAsync(cancellationToken);

        return ResponseCatalog.Job.Created
            .As<Guid>()
            .WithData(job.Id)
            .ToOperationResult();
    }
}
