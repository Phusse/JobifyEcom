using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Jobify.Ecom.Application.CQRS.Decorators;
using Jobify.Ecom.Application.CQRS.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Jobify.Ecom.Infrastructure.CQRS.Messaging;

internal class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private static readonly ConcurrentDictionary<(Type MessageType, Type ResultType), object> _cache = new();

    public Task<TResult> Send<TResult>(IRequest<TResult> message, CancellationToken cancellationToken = default)
    {
        (Type MessageType, Type ResultType) key = (message.GetType(), typeof(TResult));

        var executor = (Func<IServiceProvider, IRequest<TResult>, CancellationToken, Task<TResult>>)
            _cache.GetOrAdd(key, _ => BuildExecutorWithPipeline<TResult>(key.MessageType));

        return executor(serviceProvider, message, cancellationToken);
    }

    private static Func<IServiceProvider, IRequest<TResult>, CancellationToken, Task<TResult>> BuildExecutorWithPipeline<TResult>(Type messageType)
    {
        // Concrete handler type
        Type handlerType = typeof(IHandler<,>).MakeGenericType(messageType, typeof(TResult));

        // Lambda parameters
        ParameterExpression providerParam = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
        ParameterExpression messageParam = Expression.Parameter(typeof(IRequest<TResult>), "message");
        ParameterExpression ctParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        // Get handler from DI
        MethodCallExpression getHandlerCall = Expression.Call(
            typeof(ServiceProviderServiceExtensions),
            nameof(ServiceProviderServiceExtensions.GetRequiredService),
            [handlerType],
            providerParam
        );

        // Cast message to concrete type
        Expression castMessage = Expression.Convert(messageParam, messageType);

        // Get concrete Handle method
        MethodInfo handleMethod = handlerType.GetMethod(nameof(IHandler<,>.Handle))!;

        // Call Handle
        MethodCallExpression baseHandleCall = Expression.Call(
            Expression.Convert(getHandlerCall, handlerType),
            handleMethod,
            castMessage,
            ctParam
        );

        // Call BuildPipeline<TRequest, TResult>
        MethodInfo buildPipelineMethod = typeof(Mediator)
            .GetMethod(nameof(BuildPipeline), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(messageType, typeof(TResult));

        MethodCallExpression pipelineCall = Expression.Call(
            buildPipelineMethod,
            providerParam,
            castMessage,
            baseHandleCall,
            ctParam
        );

        // Compile lambda
        var lambda = Expression.Lambda<Func<IServiceProvider, IRequest<TResult>, CancellationToken, Task<TResult>>>(
            pipelineCall,
            providerParam,
            messageParam,
            ctParam
        );

        return lambda.Compile();
    }

    private static async Task<TResult> BuildPipeline<TRequest, TResult>(IServiceProvider serviceProvider, TRequest request, Task<TResult> finalHandlerTask, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResult>
    {
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResult>>();
        Func<Task<TResult>> next = () => finalHandlerTask;

        foreach (IPipelineBehavior<TRequest, TResult> behavior in behaviors.Reverse())
        {
            Func<Task<TResult>> capturedNext = next;
            next = () => behavior.Handle(request, capturedNext, cancellationToken);
        }

        return await next();
    }
}
