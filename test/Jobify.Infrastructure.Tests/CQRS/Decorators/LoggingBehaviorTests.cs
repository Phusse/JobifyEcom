using Jobify.Infrastructure.CQRS.Decorators;
using Jobify.Infrastructure.Tests.Utilities.Logging;
using Jobify.Infrastructure.Tests.Utilities.Messaging;
using Microsoft.Extensions.Logging;

namespace Jobify.Infrastructure.Tests.CQRS.Decorators;

public class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_Should_Call_Next_And_Return_Response()
    {
        TestLogger<LoggingBehavior<TestRequest, string>> logger = new();
        LoggingBehavior<TestRequest, string> behavior = new(logger);
        TestRequest request = new();

        bool nextCalled = false;

        Task<string> Next()
        {
            nextCalled = true;
            return Task.FromResult("OK");
        }

        string result = await behavior.Handle(request, Next, CancellationToken.None);

        Assert.True(nextCalled);
        Assert.Equal("OK", result);
    }

    [Fact]
    public async Task Handle_Should_Log_Handling_And_Handled_Information()
    {
        TestLogger<LoggingBehavior<TestRequest, string>> logger = new();
        LoggingBehavior<TestRequest, string> behavior = new(logger);

        static Task<string> Next() => Task.FromResult("OK");

        await behavior.Handle(new TestRequest(), Next);

        Assert.Contains(logger.Logs, log =>
            log.Level is LogLevel.Information && log.Message.Contains("Handling")
        );

        Assert.Contains(logger.Logs, log =>
            log.Level is LogLevel.Information && log.Message.Contains("Handled")
        );
    }

    [Fact]
    public async Task Handle_Should_Log_Warning_For_Slow_Request()
    {
        TestLogger<LoggingBehavior<TestRequest, string>> logger = new();
        LoggingBehavior<TestRequest, string> behavior = new(logger);

        static async Task<string> Next()
        {
            await Task.Delay(600);
            return "OK";
        }

        string result = await behavior.Handle(new TestRequest(), Next);

        Assert.Equal("OK", result);

        Assert.Contains(logger.Logs, log =>
            log.Level is LogLevel.Warning && log.Message.Contains("Slow request detected")
        );
    }

    [Fact]
    public async Task Handle_Should_Always_Log_Handled_Even_When_Exception_Is_Thrown()
    {
        TestLogger<LoggingBehavior<TestRequest, string>> logger = new();
        LoggingBehavior<TestRequest, string> behavior = new(logger);

        static Task<string> Next() => throw new InvalidOperationException("Boom");

        await Assert.ThrowsAsync<InvalidOperationException>(() => behavior.Handle(new TestRequest(), Next));

        Assert.Contains(logger.Logs, log =>
            log.Level is LogLevel.Information && log.Message.Contains("Handled")
        );
    }
}
