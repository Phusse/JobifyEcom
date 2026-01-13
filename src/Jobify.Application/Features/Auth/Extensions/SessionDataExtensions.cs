using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Auth.Extensions;

internal static class SessionDataExtensions
{
    extension(SessionData sessionData)
    {
        public SessionTimestampsResponse ToTimestampsResponse()
        {
            return new SessionTimestampsResponse(
                sessionData.ExpiresAt,
                sessionData.AbsoluteExpiresAt
            );
        }
    }
}
