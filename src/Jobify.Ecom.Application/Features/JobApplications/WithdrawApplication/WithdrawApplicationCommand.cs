using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.JobApplications.WithdrawApplication;

public record WithdrawApplicationCommand(
    Guid ApplicationId,
    Guid? ApplicantUserId
) : IMessage<OperationResult<object>>;
