using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Jobify.Api.Constants.Cookies;
using Jobify.Api.Models;
using Jobify.Application.Models;
using Jobify.Application.Services;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Jobify.Api.Extensions.ReverseProxy;

public static class ReverseProxyExtensions
{
    private const string InternalSessionHeader = "X-Internal-Session";

    extension(TransformBuilderContext builderContext)
    {
        public TransformBuilderContext AddInternalSessionAuth()
        {
            builderContext.AddRequestTransform(async context =>
            {
                context.ProxyRequest.Headers.Remove(InternalSessionHeader);
                HttpContext httpContext = context.HttpContext;

                if (!httpContext.Request.Cookies.TryGetValue(CookieKeys.Session, out string? rawSessionId))
                    return;

                if (!Guid.TryParse(rawSessionId, out Guid sessionId))
                    return;

                var sessionService = httpContext.RequestServices
                    .GetRequiredService<SessionManagementService>();

                SessionData? sessionData = await sessionService.GetSessionDataAsync(sessionId, httpContext.RequestAborted);

                if (sessionData is null || sessionData.IsLocked || sessionData.IsExpired())
                    return;

                JsonSerializerOptions jsonOptions = httpContext.RequestServices
                    .GetRequiredService<IOptions<JsonOptions>>()
                    .Value.SerializerOptions;

                InternalSessionData sessionDto = new(sessionData.UserId, sessionData.Role);

                string json = JsonSerializer.Serialize(sessionDto, jsonOptions);
                byte[] payloadBytes = Encoding.UTF8.GetBytes(json);

                string payload = WebEncoders.Base64UrlEncode(payloadBytes);

                string privateKeyPem = httpContext.RequestServices
                    .GetRequiredService<IConfiguration>()["InternalSessionAuth:PrivateKeyPem"]
                    ?? throw new InvalidOperationException("Missing RSA private key");

                using RSA rsa = RSA.Create();
                rsa.ImportFromPem(privateKeyPem);

                byte[] signatureBytes = rsa.SignData(
                    payloadBytes,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1
                );

                string signature = WebEncoders.Base64UrlEncode(signatureBytes);

                context.ProxyRequest.Headers.Add(InternalSessionHeader, $"{payload}.{signature}");
            });

            return builderContext;
        }
    }
}
