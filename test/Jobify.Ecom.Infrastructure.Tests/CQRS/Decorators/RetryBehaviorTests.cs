using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Exceptions;
using Jobify.Ecom.Infrastructure.CQRS.Decorators;
using Microsoft.Extensions.Logging;

namespace Jobify.Ecom.Infrastructure.Tests.CQRS.Decorators;

public class RetryBehaviorTests
{
    private class TestLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Logs { get; } = [];

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Logs.Add((logLevel, formatter(state, exception)));
        }

        private class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
    
    private sealed record TestRequest : IRequest<string>;

    [Fact]
    public async Task Handle_Should_Retry_And_Eventually_Succeed()
    {
        var logger = new TestLogger<RetryBehavior<TestRequest, string>>();
        var behavior = new RetryBehavior<TestRequest, string>(logger);
        var request = new TestRequest();

        var attempts = 0;

        Task<string> Next()
        {
            attempts++;
            if (attempts < 3)
                throw new Exception("Transient");

            return Task.FromResult("OK");
        }

        var result = await behavior.Handle(request, Next, CancellationToken.None);

        Assert.Equal(3, attempts);
        Assert.Equal("OK", result);

        Assert.Contains(logger.Logs, log =>
            log.Level == LogLevel.Warning &&
            log.Message.Contains("Retrying"));
    }

    [Fact]
    public async Task Handle_Should_Not_Retry_AppException()
    {
        var logger = new TestLogger<RetryBehavior<TestRequest, string>>();
        var behavior = new RetryBehavior<TestRequest, string>(logger);
        var request = new TestRequest();

        var attempts = 0;

        Task<string> Next()
        {
            attempts++;
            throw new AppException("ERR", 400, "fail");
        }

        await Assert.ThrowsAsync<AppException>(() =>
            behavior.Handle(request, Next, CancellationToken.None));

        Assert.Equal(1, attempts);
        Assert.Empty(logger.Logs); // no retry logging
    }

    [Fact]
    public async Task Handle_Should_Not_Retry_On_OperationCanceledException()
    {
        var logger = new TestLogger<RetryBehavior<TestRequest, string>>();
        var behavior = new RetryBehavior<TestRequest, string>(logger);
        var request = new TestRequest();

        var attempts = 0;

        Task<string> Next()
        {
            attempts++;
            throw new OperationCanceledException();
        }

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            behavior.Handle(request, Next, CancellationToken.None));

        Assert.Equal(1, attempts);
        Assert.Empty(logger.Logs); // no retry logging
    }

    [Fact]
    public async Task Handle_Should_Stop_When_CancellationToken_Is_Canceled()
    {
        var logger = new TestLogger<RetryBehavior<TestRequest, string>>();
        var behavior = new RetryBehavior<TestRequest, string>(logger);
        var request = new TestRequest();

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            behavior.Handle(request, () => Task.FromResult("OK"), cts.Token));

        Assert.Empty(logger.Logs);
    }
}
