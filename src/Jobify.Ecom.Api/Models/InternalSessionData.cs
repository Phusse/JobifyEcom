using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Api.Models;

public record InternalSessionData(
    Guid UserId,
    SystemRole Role
);
