using Jobify.Application.CQRS.Messaging;
using Jobify.Application.CQRS.Decorators;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Jobify.Infrastructure.CQRS.Decorators;

internal class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int SlowRequestThresholdMs = 500;

    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        string requestName = typeof(TRequest).Name;

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Handling {RequestName}", requestName);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            return await next();
        }
        finally
        {
            stopwatch.Stop();

            long elapsedMs = stopwatch.ElapsedMilliseconds;

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMs} ms", requestName, elapsedMs
                );

            if (elapsedMs > SlowRequestThresholdMs && logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning(
                    "Slow request detected: {RequestName} took {ElapsedMs} ms", requestName, elapsedMs
                );
            }
        }
    }
}
