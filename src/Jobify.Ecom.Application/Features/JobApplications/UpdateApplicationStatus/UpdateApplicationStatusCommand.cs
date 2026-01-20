using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Application.Features.JobApplications.UpdateApplicationStatus;

public record UpdateApplicationStatusCommand(
    Guid ApplicationId,
    JobApplicationStatus NewStatus,
    Guid? RequestingUserId
) : IMessage<OperationResult<object>>;
