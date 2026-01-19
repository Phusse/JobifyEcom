using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Application.Features.Jobs.UpdateJob;

public record UpdateJobCommand(
    Guid JobId,
    string? Title,
    string? Description,
    JobType? JobType,
    decimal? MinSalary,
    decimal? MaxSalary,
    DateTime? ClosingDate,
    Guid? UpdatedByUserId
) : IMessage<OperationResult<object>>;
