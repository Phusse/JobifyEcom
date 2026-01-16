using Jobify.Domain.Enums;

namespace Jobify.Api.Models;

internal record InternalSessionData(
    Guid UserId,
    SystemRole Role
);
