namespace Jobify.Application.Features.Auth.Models;

public record SessionTimestampsResponse(
    DateTime ExpiresAt,
    DateTime AbsoluteExpiresAt
);
