using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Api.Endpoints.Jobs.Handlers.UpdateJob;

internal record UpdateJobRequest(
    string? Title,
    string? Description,
    JobType? JobType,
    decimal? MinSalary,
    decimal? MaxSalary,
    DateTime? ClosingDate
);
