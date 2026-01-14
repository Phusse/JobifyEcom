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
        builderContext.AddRequestTransform(async transformContext =>
        {
            var httpContext = transformContext.HttpContext;

            // 1. Get session ID from cookie
            if (!httpContext.Request.Cookies.TryGetValue("jb_session_id", out string? rawSessionId)
                || !Guid.TryParse(rawSessionId, out var sessionId))
                return; // No session, skip header

            // 2. Resolve session data via service
            var sessionService = httpContext.RequestServices.GetRequiredService<SessionManagementService>();
            SessionData? sessionData = await sessionService.GetSessionDataAsync(sessionId, transformContext.HttpContext.RequestAborted);

            if (sessionData is null || sessionData.IsExpired() || sessionData.IsRevoked || sessionData.IsLocked)
                return; // Invalid session, skip header

            // 3. Serialize and Base64 encode
            var jsonOptions = httpContext.RequestServices
                .GetRequiredService<IOptions<JsonOptions>>()
                .Value.SerializerOptions;

            var json = JsonSerializer.Serialize(sessionData, jsonOptions);
            var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            // 4. Sign using secret key
            var secretKey = Encoding.UTF8.GetBytes(httpContext.RequestServices
                .GetRequiredService<IConfiguration>()["SessionSigningKey"]
                ?? throw new InvalidOperationException("Missing session signing key in configuration.")
            );

            using var hmac = new HMACSHA256(secretKey);
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));

            var token = $"{payload}.{signature}";

            // 5. Add header to downstream request
            transformContext.ProxyRequest.Headers.Add("X-Internal-Session", token);
        });
    }
);

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
