using Jobify.Application.Exceptions;
using Jobify.Infrastructure.CQRS.Decorators;
using Jobify.Infrastructure.Tests.Utilities.Logging;
using Jobify.Infrastructure.Tests.Utilities.Messaging;
using Microsoft.Extensions.Logging;

namespace Jobify.Infrastructure.Tests.CQRS.Decorators;

public class RetryBehaviorTests
{
    [Fact]
    public async Task Handle_Should_Retry_And_Eventually_Succeed()
    {
        TestLogger<RetryBehavior<TestRequest, string>> logger = new();
        RetryBehavior<TestRequest, string> behavior = new(logger);
        TestRequest request = new();

        int attempts = 0;

        Task<string> Next()
        {
            attempts++;

            if (attempts < 3)
                throw new Exception("Transient");

            return Task.FromResult("OK");
        }

        string result = await behavior.Handle(request, Next, CancellationToken.None);

        Assert.Equal(3, attempts);
        Assert.Equal("OK", result);

        Assert.Contains(logger.Logs, log =>
            log.Level is LogLevel.Warning && log.Message.Contains("Retrying")
        );
    }

    [Fact]
    public async Task Handle_Should_Not_Retry_AppException()
    {
        TestLogger<RetryBehavior<TestRequest, string>> logger = new();
        RetryBehavior<TestRequest, string> behavior = new(logger);
        TestRequest request = new();

        int attempts = 0;

        Task<string> Next()
        {
            attempts++;
            throw new AppException("ERR", 400, "fail");
        }

        await Assert.ThrowsAsync<AppException>(() =>
            behavior.Handle(request, Next, CancellationToken.None)
        );

        Assert.Equal(1, attempts);
        Assert.Empty(logger.Logs);
    }

    [Fact]
    public async Task Handle_Should_Not_Retry_On_OperationCanceledException()
    {
        TestLogger<RetryBehavior<TestRequest, string>> logger = new();
        RetryBehavior<TestRequest, string> behavior = new(logger);
        TestRequest request = new();

        int attempts = 0;

        Task<string> Next()
        {
            attempts++;
            throw new OperationCanceledException();
        }

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            behavior.Handle(request, Next, CancellationToken.None)
        );

        Assert.Equal(1, attempts);
        Assert.Empty(logger.Logs);
    }

    [Fact]
    public async Task Handle_Should_Stop_When_CancellationToken_Is_Canceled()
    {
        TestLogger<RetryBehavior<TestRequest, string>> logger = new();
        RetryBehavior<TestRequest, string> behavior = new(logger);
        TestRequest request = new();

        using CancellationTokenSource cts = new();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            behavior.Handle(request, () => Task.FromResult("OK"), cts.Token)
        );

        Assert.Empty(logger.Logs);
    }
}
