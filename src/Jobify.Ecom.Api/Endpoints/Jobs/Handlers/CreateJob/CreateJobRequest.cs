using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Api.Endpoints.Jobs.Handlers.CreateJob;

internal record CreateJobRequest(
    string Title,
    string Description,
    JobType JobType,
    decimal MinSalary,
    decimal MaxSalary,
    DateTime ClosingDate
);
