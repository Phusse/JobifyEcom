namespace Jobify.Application.Features.Auth.Models;

public record SessionResult(
    Guid SessionId,
    SessionTimestampsResponse Timestamps
);
