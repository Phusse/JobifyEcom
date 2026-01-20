using Jobify.Ecom.Application.Features.JobApplications.Models;
using Jobify.Ecom.Domain.Entities.JobApplications;

namespace Jobify.Ecom.Application.Features.JobApplications.Extensions;

public static class JobApplicationExtensions
{
    extension(JobApplication application)
    {
        public JobApplicationResponse ToResponse() => new(
            application.Id,
            application.JobId,
            application.ApplicantUserId,
            application.Status,
            application.CreatedAt,
            application.UpdatedAt
        );
    }
}
