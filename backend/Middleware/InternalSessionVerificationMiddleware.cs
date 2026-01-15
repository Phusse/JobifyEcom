using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace JobifyEcom.Middleware;

public class InternalSessionVerificationMiddleware
{
    private readonly RequestDelegate _next;

    public InternalSessionVerificationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration config)
    {
        if (context.Request.Headers.TryGetValue("X-Internal-Session", out var headerValue))
        {
            Console.WriteLine($"X-Internal-Session received: {headerValue}");

            try
            {
                var parts = headerValue.ToString().Split('.');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Invalid token format (expected payload.signature)");
                    await _next(context);
                    return;
                }

                // Decode payload and signature
                var payloadBytes = WebEncoders.Base64UrlDecode(parts[0]);
                var payloadJson = Encoding.UTF8.GetString(payloadBytes); // Use string for JSON deserialization
                var signatureBytes = WebEncoders.Base64UrlDecode(parts[1]);

                // Load RSA public key
                var publicKeyPem = config["InternalAuth:PublicKeyPem"]
                    ?? throw new InvalidOperationException("Missing RSA public key");

                using var rsa = RSA.Create();
                rsa.ImportFromPem(publicKeyPem);

                // Verify signature
                if (!rsa.VerifyData(payloadBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                {
                    Console.WriteLine("Invalid signature!");
                    await _next(context);
                    return;
                }

                Console.WriteLine("Signature valid ✅");

                // Deserialize JSON (case-insensitive)
                var session = JsonSerializer.Deserialize<SessionData>(
                    payloadJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (session == null)
                {
                    Console.WriteLine("Failed to deserialize session payload");
                    await _next(context);
                    return;
                }

                if (session.IsExpired())
                {
                    Console.WriteLine("Session has expired!");
                    await _next(context);
                    return;
                }

                Console.WriteLine("Session valid:");
                Console.WriteLine(JsonSerializer.Serialize(session, new JsonSerializerOptions { WriteIndented = true }));

                // Optional: attach claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
                    new Claim(ClaimTypes.Role, session.Role.ToString())
                };

                context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "InternalSession"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception verifying X-Internal-Session: {ex.Message}");
            }
        }

        await _next(context);
    }
}

// SessionData class remains the same, no changes needed
public class SessionData
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime AbsoluteExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public bool RememberMe { get; set; }
    public bool IsLocked { get; set; }
    public string Role { get; set; }

    [JsonConstructor]
    public SessionData(
        Guid sessionId,
        Guid userId,
        DateTime expiresAt,
        DateTime absoluteExpiresAt,
        bool isRevoked,
        bool rememberMe,
        bool isLocked,
        string role)
    {
        SessionId = sessionId;
        UserId = userId;
        ExpiresAt = DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc);
        AbsoluteExpiresAt = DateTime.SpecifyKind(absoluteExpiresAt, DateTimeKind.Utc);
        IsRevoked = isRevoked;
        RememberMe = rememberMe;
        IsLocked = isLocked;
        Role = role;
    }

    public bool IsExpired()
    {
        var now = DateTime.UtcNow;
        return now >= ExpiresAt || now >= AbsoluteExpiresAt || IsRevoked;
    }
}
