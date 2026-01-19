using Jobify.Application.CQRS.Messaging;
using Jobify.Application.CQRS.Decorators;
using Jobify.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jobify.Infrastructure.CQRS.Decorators;

internal class RetryBehavior<TRequest, TResponse>(ILogger<RetryBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage<TResponse>
{
    private const int MaxRetries = 3;
    private static readonly TimeSpan BaseDelay = TimeSpan.FromMilliseconds(100);

    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        Exception? lastException = null;

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await next();
            }
            catch (AppException) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) when (IsTransient(ex) && attempt < MaxRetries)
            {
                lastException = ex;

                if (logger.IsEnabled(LogLevel.Warning))
                    logger.LogWarning(
                        ex,
                        "Transient failure. Retrying {Attempt}/{MaxRetries} for {Request}",
                        attempt,
                        MaxRetries,
                        typeof(TRequest).Name
                    );

                await Task.Delay(
                    TimeSpan.FromMilliseconds(BaseDelay.TotalMilliseconds * attempt),
                    cancellationToken
                );
            }
            catch (Exception) { throw; }
        }

        // This line is theoretically unreachable, but kept for correctness
        throw lastException!;
    }

    private static bool IsTransient(Exception ex) =>
        ex is TimeoutException
        || ex is TaskCanceledException
        || ex is DbUpdateConcurrencyException
        || (ex is DbUpdateException dbEx && dbEx.InnerException is TimeoutException);
}
