using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.UpdateApplicationStatus;

public record UpdateApplicationStatusRequest(
    JobApplicationStatus NewStatus
);
