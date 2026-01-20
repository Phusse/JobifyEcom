using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.JobApplications.Models;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.JobApplications.GetMyApplications;

public record GetMyApplicationsQuery(
    Guid? ApplicantUserId,
    int PageSize = 10,
    DateTime? LastCreatedAt = null,
    Guid? LastApplicationId = null
) : IMessage<OperationResult<IReadOnlyList<JobApplicationResponse>>>;
