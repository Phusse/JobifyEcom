using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Features.Jobs.Extensions;
using Jobify.Ecom.Application.Features.Jobs.Models;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.Jobs.GetJobs;

internal sealed class GetJobsRequestHandler(AppDbContext context)
    : IHandler<GetJobsRequest, OperationResult<IEnumerable<JobResponse>>>
{
    public async Task<OperationResult<IEnumerable<JobResponse>>> Handle(GetJobsRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Job> query = context.Jobs
            .AsNoTracking()
            .OrderByDescending(j => j.CreatedAt)
            .ThenByDescending(j => j.Id); // tie-breaker

        if (request.LastCreatedAt.HasValue && request.LastJobId.HasValue)
        {
            query = query.Where(j =>
                j.CreatedAt < request.LastCreatedAt.Value ||
                (j.CreatedAt == request.LastCreatedAt.Value && j.Id < request.LastJobId.Value)
            );
        }

        var jobs = await query
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var response = jobs.Select(j => j.ToResponse());

        return ResponseCatalog.Job.JobsRetrieved
            .As<IEnumerable<JobResponse>>()
            .WithData(response)
            .ToOperationResult();
    }
}
