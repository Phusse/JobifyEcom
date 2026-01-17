using Jobify.Ecom.Application.Features.Jobs.Models;
using Jobify.Ecom.Domain.Entities.Jobs;

namespace Jobify.Ecom.Application.Features.Jobs.Extensions;

public static class JobExtensions
{
    public static JobResponse ToResponse(this Job job) =>
        new(
            job.Id,
            job.PostedByUserId,
            job.Title,
            job.Description,
            job.JobType,
            job.MinSalary,
            job.MaxSalary,
            job.ClosingDate,
            job.CreatedAt,
            job.UpdatedAt
        );
}
