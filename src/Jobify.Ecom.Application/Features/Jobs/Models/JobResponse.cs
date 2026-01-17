using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Application.Features.Jobs.Models;

public record JobResponse(
    Guid Id,
    Guid PostedByUserId,
    string Title,
    string Description,
    JobType JobType,
    decimal MinSalary,
    decimal MaxSalary,
    DateTime ClosingDate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
