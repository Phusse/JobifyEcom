using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.JobApplications.Models;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.JobApplications.GetApplicationsForJob;

public record GetApplicationsForJobQuery(
    Guid JobId,
    Guid? RequestingUserId,
    int PageSize = 10,
    DateTime? LastCreatedAt = null,
    Guid? LastApplicationId = null
) : IMessage<OperationResult<IReadOnlyList<JobApplicationResponse>>>;
