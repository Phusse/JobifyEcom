using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Jobify.Ecom.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Jobify.Ecom.Api.Authentication;

internal class SessionAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IConfiguration config)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Endpoint? endpoint = Context.GetEndpoint();

        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is not null)
            return AuthenticateResult.NoResult();

        if (!Request.Headers.TryGetValue("X-Internal-Session", out var headerValue))
            return AuthenticateResult.NoResult();

        try
        {
            string[] parts = headerValue.ToString().Split('.');

            if (parts.Length is not 2)
                return AuthenticateResult.Fail("Invalid token format");

            byte[] payloadBytes = WebEncoders.Base64UrlDecode(parts[0]);
            byte[] signatureBytes = WebEncoders.Base64UrlDecode(parts[1]);
            string payloadJson = Encoding.UTF8.GetString(payloadBytes);

            string publicKeyPem = config["GatewaySessionVerificationKey"]
                ?? throw new InvalidOperationException("GatewaySessionVerificationKey is in the config.");

            using RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);

            if (!rsa.VerifyData(payloadBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                return AuthenticateResult.Fail("Invalid signature");

            var session = JsonSerializer.Deserialize<SessionData>(payloadJson, JsonOptions);

            if (session is null)
                return AuthenticateResult.Fail("Invalid session data");

            IEnumerable<Claim> claims = [
                new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString("N")),
                new Claim(ClaimTypes.Role, session.Role),
            ];

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
