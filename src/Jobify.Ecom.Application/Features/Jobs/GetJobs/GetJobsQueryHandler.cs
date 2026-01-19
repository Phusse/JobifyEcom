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

public class GetJobsQueryHandler(AppDbContext context) : IHandler<GetJobsQuery, OperationResult<IReadOnlyList<JobResponse>>>
{
    public async Task<OperationResult<IReadOnlyList<JobResponse>>> Handle(GetJobsQuery message, CancellationToken cancellationToken = default)
    {
        IQueryable<Job> query = context.Jobs
            .AsNoTracking()
            .OrderByDescending(j => j.AuditState.CreatedAt)
            .ThenByDescending(j => j.Id);

        if (message.LastCreatedAt.HasValue && message.LastJobId.HasValue)
        {
            DateTime lastCreatedAt = message.LastCreatedAt.Value;
            Guid lastJobId = message.LastJobId.Value;

            query = query.Where(j =>
                j.AuditState.CreatedAt < lastCreatedAt
                || (j.AuditState.CreatedAt == lastCreatedAt && j.Id.CompareTo(lastJobId) < 0)
            );
        }

        List<JobResponse> response = await query
            .Take(message.PageSize)
            .Select(j => j.ToResponse())
            .ToListAsync(cancellationToken);

        return ResponseCatalog.Job.Retrieved
            .As<IReadOnlyList<JobResponse>>()
            .WithData(response)
            .ToOperationResult();
    }
}
