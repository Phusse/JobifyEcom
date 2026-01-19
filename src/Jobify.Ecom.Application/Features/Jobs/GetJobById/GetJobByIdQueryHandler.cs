using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Features.Jobs.Extensions;
using Jobify.Ecom.Application.Features.Jobs.Models;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.Jobs.GetJobById;

public class GetJobByIdQueryHandler(AppDbContext context) : IHandler<GetJobByIdQuery, OperationResult<JobResponse>>
{
    public async Task<OperationResult<JobResponse>> Handle(GetJobByIdQuery message, CancellationToken cancellationToken = default)
    {
        Job job = await context.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == message.JobId, cancellationToken)
            ?? throw ResponseCatalog.Job.NotFound.ToException();

        JobResponse response = job.ToResponse();

        return ResponseCatalog.Job.Found
            .As<JobResponse>()
            .WithData(response)
            .ToOperationResult();
    }
}
