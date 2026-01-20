using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Application.Features.JobApplications.Models;

public record JobApplicationResponse(
    Guid Id,
    Guid JobId,
    Guid ApplicantUserId,
    JobApplicationStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
