using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Auth.Extensions;

internal static class SessionDataExtensions
{
    extension(SessionData sessionData)
    {
        public SessionTimestampsResponse ToTimestampsResponse() => new(
            sessionData.ExpiresAt,
            sessionData.AbsoluteExpiresAt
        );
    }
}
