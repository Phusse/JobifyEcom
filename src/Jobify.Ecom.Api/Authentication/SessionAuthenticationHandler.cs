using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Jobify.Ecom.Api.Constants.Auth;
using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Jobify.Ecom.Api.Authentication;

internal class SessionAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IConfiguration configuration, UserIdentityService userIdentityService) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Endpoint? endpoint = Context.GetEndpoint();

        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is not null)
            return AuthenticateResult.NoResult();

        if (!Request.Headers.TryGetValue("X-Internal-Session", out StringValues headerValue))
            return AuthenticateResult.NoResult();

        try
        {
            string[] parts = headerValue.ToString().Split('.');

            if (parts.Length is not 2)
                return AuthenticateResult.Fail("Invalid token format");

            byte[] payloadBytes = WebEncoders.Base64UrlDecode(parts[0]);
            byte[] signatureBytes = WebEncoders.Base64UrlDecode(parts[1]);
            string payloadJson = Encoding.UTF8.GetString(payloadBytes);

            string publicKeyPem = configuration["GatewaySessionVerificationKey"]
                ?? throw new InvalidOperationException("GatewaySessionVerificationKey is missing in config.");

            using RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);

            if (!rsa.VerifyData(payloadBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                return AuthenticateResult.Fail("Invalid signature");

            SessionData? session = JsonSerializer.Deserialize<SessionData>(payloadJson, JsonOptions);

            if (session is null)
                return AuthenticateResult.Fail("Invalid session data");

            List<Claim> claims = [
                new Claim(SessionClaimTypes.ExternalUserId, session.UserId.ToString("N")),
                new Claim(ClaimTypes.Role, session.Role)
            ];

            UserIdentity? userIdentity = await userIdentityService.GetBySourceUserIdAsync(session.UserId);

            if (userIdentity is not null)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userIdentity.UserId.ToString("N")));

            ClaimsIdentity identity = new(claims, Scheme.Name);
            ClaimsPrincipal principal = new(identity);
            AuthenticationTicket ticket = new(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating InternalSession header");
            return AuthenticateResult.Fail("Exception validating session");
        }
    }
}
