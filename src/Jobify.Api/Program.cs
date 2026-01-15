using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jobify.Api;
using Jobify.Api.Authentication;
using Jobify.Api.Constants.Auth;
using Jobify.Api.Endpoints.Auth;
using Jobify.Api.Endpoints.Base;
using Jobify.Api.Endpoints.Users;
using Jobify.Api.Extensions.OpenApi;
using Jobify.Api.Middleware;
using Jobify.Application;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Models;
using Jobify.Application.Services;
using Jobify.Domain.Enums;
using Jobify.Infrastructure;
using Jobify.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Yarp.ReverseProxy.Transforms;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration, [typeof(IMediator).Assembly]);
builder.Services.AddApiServices();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(AuthenticationSchemes.Session)
    .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(
        AuthenticationSchemes.Session,
        options => { options.ClaimsIssuer = "Jobify"; }
    );

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthenticationResultHandler>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserOnly", p => p.RequireRole(SystemRole.User.ToString()))
    .AddPolicy("AdminOnly", p => p.RequireRole(SystemRole.Admin.ToString(), SystemRole.SuperAdmin.ToString()))
    .AddPolicy("SuperAdminOnly", p => p.RequireRole(SystemRole.SuperAdmin.ToString()));

builder.Services.Configure<JsonOptions>(opts =>
{
    JsonSerializerOptions serializer = opts.SerializerOptions;

    serializer.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    serializer.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    serializer.WriteIndented = true;
    serializer.Converters.Add(new JsonStringEnumConverter());
});


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(async ctx =>
        {
            // ALWAYS strip client-sent header
            ctx.ProxyRequest.Headers.Remove("X-Internal-Session");

            var httpContext = ctx.HttpContext;

            // Read session cookie
            if (!httpContext.Request.Cookies.TryGetValue("jb_session_id", out var rawSessionId)
                || !Guid.TryParse(rawSessionId, out var sessionId))
                return;

            var sessionService = httpContext.RequestServices
                .GetRequiredService<SessionManagementService>();

            var session = await sessionService.GetSessionDataAsync(
                sessionId,
                httpContext.RequestAborted);

            if (session is null || session.IsExpired() || session.IsRevoked || session.IsLocked)
                return;

            // Serialize session
            var jsonOptions = httpContext.RequestServices
                .GetRequiredService<IOptions<JsonOptions>>()
                .Value.SerializerOptions;

            var json = JsonSerializer.Serialize(session, jsonOptions);
            var payloadBytes = Encoding.UTF8.GetBytes(json);

            // Base64URL encode payload
            var payload = WebEncoders.Base64UrlEncode(payloadBytes);

            // Load PRIVATE key
            var privateKeyPem = httpContext.RequestServices
                .GetRequiredService<IConfiguration>()["InternalAuth:PrivateKeyPem"]
                ?? throw new InvalidOperationException("Missing RSA private key");

            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);

            // Sign RAW payload bytes
            var signatureBytes = rsa.SignData(
                payloadBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            var signature = WebEncoders.Base64UrlEncode(signatureBytes);
            var token = $"{payload}.{signature}";

            ctx.ProxyRequest.Headers.Add("X-Internal-Session", token);
        });
    });

builder.Services.AddOpenApi(options =>
{
    options.AddCustomOpenApiTransformer();
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Title = "Jobify API Gateway";
        options.DefaultHttpClient = new(ScalarTarget.Node, ScalarClient.Fetch);
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("Local5122");

app.UseMiddleware<TraceIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SessionRefreshMiddleware>();

app.MapReverseProxy();

app.UseAuthentication();
app.UseAuthorization();

app.MapBaseEndpoints();
app.MapAuthEndpoints();
app.MapUserEndpoints();

app.Run();
