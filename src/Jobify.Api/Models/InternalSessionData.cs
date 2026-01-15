using Jobify.Domain.Enums;

namespace Jobify.Api.Models;

public record InternalSessionData(
    Guid UserId,
    SystemRole Role
);
