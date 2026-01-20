using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Features.JobApplications.Extensions;
using Jobify.Ecom.Application.Features.JobApplications.Models;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.JobApplications;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.JobApplications.GetApplicationsForJob;

public class GetApplicationsForJobQueryHandler(AppDbContext context) : IHandler<GetApplicationsForJobQuery, OperationResult<IReadOnlyList<JobApplicationResponse>>>
{
    public async Task<OperationResult<IReadOnlyList<JobApplicationResponse>>> Handle(GetApplicationsForJobQuery message, CancellationToken cancellationToken = default)
    {
        if (message.RequestingUserId is not Guid requestingUserId)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        Job? job = await context.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == message.JobId, cancellationToken)
            ?? throw ResponseCatalog.Job.NotFound.ToException();

        if (job.PostedByUserId != requestingUserId)
            throw ResponseCatalog.JobApplication.ModificationForbidden.ToException();

        IQueryable<JobApplication> query = context.JobApplications
            .AsNoTracking()
            .Where(ja => ja.JobId == message.JobId)
            .OrderByDescending(ja => ja.AuditState.CreatedAt)
            .ThenByDescending(ja => ja.Id);

        if (message.LastCreatedAt.HasValue && message.LastApplicationId.HasValue)
        {
            DateTime lastCreatedAt = message.LastCreatedAt.Value;
            Guid lastApplicationId = message.LastApplicationId.Value;

            query = query.Where(ja =>
                ja.AuditState.CreatedAt < lastCreatedAt
                || (ja.AuditState.CreatedAt == lastCreatedAt && ja.Id.CompareTo(lastApplicationId) < 0)
            );
        }

        List<JobApplicationResponse> response = await query
            .Take(message.PageSize)
            .Select(ja => ja.ToResponse())
            .ToListAsync(cancellationToken);

        return ResponseCatalog.JobApplication.Retrieved
            .As<IReadOnlyList<JobApplicationResponse>>()
            .WithData(response)
            .ToOperationResult();
    }
}
