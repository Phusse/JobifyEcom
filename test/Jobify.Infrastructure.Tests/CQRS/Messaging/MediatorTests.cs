using Jobify.Application.CQRS.Decorators;
using Jobify.Application.CQRS.Messaging;
using Jobify.Infrastructure.CQRS.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Jobify.Infrastructure.Tests.CQRS.Messaging;

public class MediatorTests
{
    private static class InMemoryDb
    {
        private static readonly List<string> _events = [];

        public static void Clear() => _events.Clear();
        public static void Add(string value) => _events.Add(value);
        public static IReadOnlyList<string> Events => _events;
    }

    private record DoubleValueRequest(int Value) : IRequest<int>;

    private class DoubleValueHandler : IHandler<DoubleValueRequest, int>
    {
        public Task<int> Handle(DoubleValueRequest request, CancellationToken ct)
            => Task.FromResult(request.Value * 2);
    }

    private class ExceptionHandler : IHandler<DoubleValueRequest, int>
    {
        public Task<int> Handle(DoubleValueRequest request, CancellationToken ct)
            => throw new InvalidOperationException("Handler failure");
    }

    private record AddToMemoryCommand(string Value) : IRequest<string>;

    private class AddToMemoryHandler : IHandler<AddToMemoryCommand, string>
    {
        public Task<string> Handle(AddToMemoryCommand request, CancellationToken ct)
        {
            InMemoryDb.Add(request.Value);
            return Task.FromResult(request.Value);
        }
    }

    private class DelegateBehavior<TRequest, TResponse>(Func<TRequest, Func<Task<TResponse>>, CancellationToken, Task<TResponse>> func) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        public Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken)
            => func(request, next, cancellationToken);
    }

    private class IncrementBehavior : IPipelineBehavior<DoubleValueRequest, int>
    {
        public async Task<int> Handle(DoubleValueRequest request, Func<Task<int>> next, CancellationToken ct)
        {
            int result = await next();
            return result + 1;
        }
    }

    [Fact]
    public async Task Mediator_Should_Invoke_Handler_And_Behaviors()
    {
        ServiceCollection services = new();
        services.AddSingleton<IHandler<DoubleValueRequest, int>, DoubleValueHandler>();
        services.AddTransient<IPipelineBehavior<DoubleValueRequest, int>, IncrementBehavior>();
        ServiceProvider provider = services.BuildServiceProvider();

        Mediator mediator = new(provider);
        DoubleValueRequest request = new(Value: 5);

        int result = await mediator.Send(request);

        Assert.Equal(11, result);
    }

    [Fact]
    public async Task Mediator_Should_Propagate_Exception_From_Handler()
    {
        ServiceCollection services = new();
        services.AddSingleton<IHandler<DoubleValueRequest, int>, ExceptionHandler>();
        ServiceProvider provider = services.BuildServiceProvider();

        Mediator mediator = new(provider);
        DoubleValueRequest request = new(Value: 5);

        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(request));
    }

    [Fact]
    public async Task Mediator_Executes_Behaviors_Before_Handler_In_Order()
    {
        InMemoryDb.Clear();
        ServiceCollection services = new();
        services.AddSingleton<IHandler<AddToMemoryCommand, string>, AddToMemoryHandler>();

        services.AddTransient<IPipelineBehavior<AddToMemoryCommand, string>>(_ =>
            new DelegateBehavior<AddToMemoryCommand, string>((req, next, ct) =>
            {
                InMemoryDb.Add("First");
                return next();
            })
        );

        services.AddTransient<IPipelineBehavior<AddToMemoryCommand, string>>(_ =>
            new DelegateBehavior<AddToMemoryCommand, string>((req, next, ct) =>
            {
                InMemoryDb.Add("Second");
                return next();
            })
        );

        ServiceProvider provider = services.BuildServiceProvider();
        Mediator mediator = new(provider);

        string result = await mediator.Send(new AddToMemoryCommand("Handler"));

        Assert.Equal("Handler", result);
        Assert.Equal(["First", "Second", "Handler"], InMemoryDb.Events);
    }
}
