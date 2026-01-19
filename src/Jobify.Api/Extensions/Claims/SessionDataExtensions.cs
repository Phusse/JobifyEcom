using System.Security.Claims;
using Jobify.Application.Models;
using Jobify.Api.Constants.Auth;

namespace Jobify.Api.Extensions.Claims;

internal static class SessionDataExtensions
{
    extension(SessionData session)
    {
        public IEnumerable<Claim> ToClaims()
        {
            yield return new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString("N"));
            yield return new Claim(ClaimTypes.Role, session.Role.ToString());
            yield return new Claim(SessionClaimTypes.SessionId, session.SessionId.ToString("N"));
        }
    }
}
