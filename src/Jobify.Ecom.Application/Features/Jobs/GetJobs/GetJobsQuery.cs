using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.Models;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.Jobs.GetJobs;

public record GetJobsQuery(
    int PageSize = 10,
    DateTime? LastCreatedAt = null,
    Guid? LastJobId = null
) : IMessage<OperationResult<IReadOnlyList<JobResponse>>>;
