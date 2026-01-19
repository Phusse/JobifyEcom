using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.Models;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.Jobs.GetJobById;

public record GetJobByIdQuery(
    Guid JobId
) : IMessage<OperationResult<JobResponse>>;
