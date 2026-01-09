using FluentAssertions;
using Jobify.Ecom.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jobify.Ecom.Api.Tests.Middleware;

public class TraceIdMiddlewareTests
{
    [Fact]
    public async Task Adds_X_Trace_Id_Header_To_Response()
    {
        using IHost host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.Configure(app =>
                {
                    app.UseMiddleware<TraceIdMiddleware>();

                    app.Run(context =>
                    {
                        return Task.CompletedTask;
                    });
                });
            })
            .StartAsync();

        HttpClient client = host.GetTestClient();

        HttpResponseMessage response = await client.GetAsync("/");

        response.Headers.Contains("X-Trace-Id").Should().BeTrue();

        string traceId = response.Headers
            .GetValues("X-Trace-Id")
            .Single();

        traceId.Should().NotBeNullOrWhiteSpace();
    }
}
