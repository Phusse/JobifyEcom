using System.Net;
using FluentAssertions;
using Jobify.Api.Middleware;
using Jobify.Application.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jobify.Api.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private static async Task<IHost> CreateHost(RequestDelegate throwingDelegate)
    {
        return await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();

                webHost.Configure(app =>
                {
                    app.UseMiddleware<ExceptionHandlingMiddleware>();

                    app.Run(throwingDelegate);
                });
            })
            .StartAsync();
    }

    [Fact]
    public async Task AppException_Returns_Configured_Status_Code_And_Json_Response()
    {
        using IHost host = await CreateHost(_ =>
            throw new AppException(id: "Test Error", statusCode: 400, title: "Test Message"));

        HttpClient client = host.GetTestClient();

        HttpResponseMessage response = await client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");

        string content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Unhandled_Exception_Returns_500_And_Json_Response()
    {
        using IHost host = await CreateHost(_ =>
            throw new InvalidOperationException("Boom"));

        HttpClient client = host.GetTestClient();

        HttpResponseMessage response = await client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");

        string content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();
    }
}
