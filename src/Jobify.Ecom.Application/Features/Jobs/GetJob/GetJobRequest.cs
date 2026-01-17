using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.Models;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.Jobs.GetJob;

public record GetJobRequest(
    Guid JobId
) : IRequest<OperationResult<JobResponse>>;
