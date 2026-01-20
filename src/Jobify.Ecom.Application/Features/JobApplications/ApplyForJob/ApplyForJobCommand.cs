using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.JobApplications.ApplyForJob;

public record ApplyForJobCommand(
    Guid JobId,
    Guid? ApplicantUserId
) : IMessage<OperationResult<Guid>>;
