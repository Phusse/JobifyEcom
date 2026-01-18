using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.Jobs.DeleteJob;

public record DeleteJobCommand(
    Guid JobId,
    Guid? DeletedByUserId
) : IMessage<OperationResult<object>>;
