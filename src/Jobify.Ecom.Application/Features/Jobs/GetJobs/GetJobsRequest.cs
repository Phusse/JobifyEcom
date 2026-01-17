using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.Models;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.Jobs.GetJobs;

public record GetJobsRequest(
    int PageSize = 10,
    DateTime? LastCreatedAt = null,
    Guid? LastJobId = null // use JobId as cursor
) : IRequest<OperationResult<IEnumerable<JobResponse>>>;