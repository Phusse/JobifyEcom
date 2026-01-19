using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Application.Features.Jobs.CreateJob;

public record CreateJobCommand(
    string Title,
    string Description,
    JobType JobType,
    decimal MinSalary,
    decimal MaxSalary,
    DateTime ClosingDate,
    Guid? PostedByUserId
) : IMessage<OperationResult<Guid>>;
