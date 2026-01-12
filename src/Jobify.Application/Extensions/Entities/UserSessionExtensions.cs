using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Models;
using Jobify.Domain.Entities.UserSessions;
using Jobify.Domain.Enums;

namespace Jobify.Application.Extensions.Entities;

internal static class UserSessionExtensions
{
    extension(UserSession session)
    {
        public SessionData ToSessionData(SystemRole role, bool isLocked)
            => new(
                SessionId: session.Id,
                UserId: session.UserId,
                ExpiresAt: session.ExpiresAt,
                AbsoluteExpiresAt: session.AbsoluteExpiresAt,
                IsRevoked: session.IsRevoked,
                RememberMe: session.RememberMe,
                IsLocked: isLocked,
                Role: role
            );

        public SessionTimestampsResponse ToTimestampsResponse()
        {
            return new SessionTimestampsResponse(
                session.ExpiresAt,
                session.AbsoluteExpiresAt
            );
        }
    }
}
