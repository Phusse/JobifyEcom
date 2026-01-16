using System.Text.Json;
using System.Text.Json.Serialization;
using Jobify.Api;
using Jobify.Api.Authentication;
using Jobify.Api.Constants.Auth;
using Jobify.Api.Endpoints.Auth;
using Jobify.Api.Endpoints.Base;
using Jobify.Api.Endpoints.Users;
using Jobify.Api.Extensions.Claims;
using Jobify.Api.Extensions.OpenApi;
using Jobify.Api.Extensions.ReverseProxy;
using Jobify.Api.Middleware;
using Jobify.Application;
using Jobify.Application.CQRS.Messaging;
using Jobify.Infrastructure;
using Jobify.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;

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

builder.Services.AddAuthorizationBuilder().AddCustomPolicies();

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
        builderContext
            .AddInternalSessionAuth()
            .AddInternalTraceId();
    });

builder.Services.AddOpenApi(options => { options.AddCustomOpenApiTransformer(); });

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

app.UseMiddleware<TraceIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SessionRefreshMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.MapBaseEndpoints();
app.MapAuthEndpoints();
app.MapUserEndpoints();

app.Run();
