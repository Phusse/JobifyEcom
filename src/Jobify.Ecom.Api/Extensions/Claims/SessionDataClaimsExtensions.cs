using System.Security.Claims;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Api.Constants.Auth;

namespace Jobify.Ecom.Api.Extensions.Claims;

internal static class SessionDataClaimsExtensions
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
