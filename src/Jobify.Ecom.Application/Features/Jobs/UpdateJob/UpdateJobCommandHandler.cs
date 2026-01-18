using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Enums;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.Jobs.UpdateJob;

public class UpdateJobCommandHandler(AppDbContext context) : IHandler<UpdateJobCommand, OperationResult<object>>
{
    public async Task<OperationResult<object>> Handle(UpdateJobCommand message, CancellationToken cancellationToken = default)
    {
        if (message.UpdatedByUserId is null)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        Job job = await context.Jobs
            .FirstOrDefaultAsync(j => j.Id == message.JobId, cancellationToken)
            ?? throw ResponseCatalog.Job.NotFound.ToException();

        if (job.PostedByUserId != message.UpdatedByUserId.Value)
            throw ResponseCatalog.Job.ModificationForbidden.ToException();

        if (message.Title is not null)
            job.UpdateTitle(message.Title);

        if (message.Description is not null)
            job.UpdateDescription(message.Description);

        if (message.JobType is not null)
            job.UpdateJobType(message.JobType.Value);

        if (message.MinSalary is not null || message.MaxSalary is not null)
        {
            decimal newMin = message.MinSalary ?? job.MinSalary;
            decimal newMax = message.MaxSalary ?? job.MaxSalary;

            if (newMin < 0 || newMax < 0 || newMax < newMin)
            {
                throw ResponseCatalog.Job.InvalidUpdate
                    .WithDetails([
                        new ResponseDetail(
                            "The salary range is invalid. Ensure that both minimum and maximum salaries are non-negative and that the maximum salary is greater than or equal to the minimum salary.",
                            ResponseSeverity.Error
                        )
                    ])
                    .ToException();
            }

            job.UpdateSalary(newMin, newMax);
        }

        if (message.ClosingDate is not null)
        {
            if (message.ClosingDate.Value <= DateTime.UtcNow)
            {
                throw ResponseCatalog.Job.InvalidUpdate
                    .WithDetails([
                        new ResponseDetail(
                            "The closing date must be in the future. Please provide a valid future date for the job closing.",
                            ResponseSeverity.Error
                        )
                    ])
                    .ToException();
            }

            job.UpdateClosingDate(message.ClosingDate.Value);
        }

        await context.SaveChangesAsync(cancellationToken);

        return ResponseCatalog.Job.Updated
            .As<object>()
            .ToOperationResult();
    }
}
