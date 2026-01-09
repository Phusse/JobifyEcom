using Jobify.Application.CQRS.Messaging;
using Jobify.Application.CQRS.Decorators;
using Microsoft.Extensions.Logging;
using Jobify.Application.Exceptions;

namespace Jobify.Infrastructure.CQRS.Decorators;

internal class RetryBehavior<TRequest, TResponse>(ILogger<RetryBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int MaxRetries = 3;

    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        int attempt = 1;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await next();
            }
            catch (AppException) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) when (attempt <= MaxRetries)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                    logger.LogWarning(
                        ex,
                        "Transient failure. Retrying {Attempt}/{MaxRetries} for {Request}",
                        attempt,
                        MaxRetries,
                        typeof(TRequest).Name
                    );

                await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt), cancellationToken);

                attempt++;
            }
        }
    }
}
