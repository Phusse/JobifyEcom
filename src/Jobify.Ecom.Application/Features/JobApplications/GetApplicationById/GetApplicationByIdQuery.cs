using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.JobApplications.Models;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.JobApplications.GetApplicationById;

public record GetApplicationByIdQuery(
    Guid ApplicationId,
    Guid? RequestingUserId
) : IMessage<OperationResult<JobApplicationResponse>>;
