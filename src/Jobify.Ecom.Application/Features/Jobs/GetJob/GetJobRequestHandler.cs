using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Features.Jobs.Extensions;
using Jobify.Ecom.Application.Features.Jobs.Models;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.Jobs.GetJob;

internal sealed class GetJobRequestHandler(AppDbContext context)
    : IHandler<GetJobRequest, OperationResult<JobResponse>>
{
    public async Task<OperationResult<JobResponse>> Handle(GetJobRequest request, CancellationToken cancellationToken)
    {
        Job job = await context.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken)
            ?? throw ResponseCatalog.Job.JobNotFound.ToException();

        JobResponse response = job.ToResponse();

        return ResponseCatalog.Job.JobFound
            .As<JobResponse>()
            .WithData(response)
            .ToOperationResult();
    }
}
