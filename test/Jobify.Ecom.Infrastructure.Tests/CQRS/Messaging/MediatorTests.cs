using Jobify.Ecom.Application.CQRS.Decorators;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Infrastructure.CQRS.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Jobify.Ecom.Infrastructure.Tests.CQRS.Messaging;

public class MediatorTests
{
    protected record TestRequest(int Value) : IRequest<int>;

    protected class TestHandler : IHandler<TestRequest, int>
    {
        public Task<int> Handle(TestRequest request, CancellationToken ct)
            => Task.FromResult(request.Value * 2);
    }

    protected class ExceptionHandler : IHandler<TestRequest, int>
    {
        public Task<int> Handle(TestRequest request, CancellationToken ct)
            => throw new InvalidOperationException("Handler failure");
    }

    protected class DelegateBehavior(Func<TestRequest, Func<Task<int>>, CancellationToken, Task<int>> func) : IPipelineBehavior<TestRequest, int>
    {
        private readonly Func<TestRequest, Func<Task<int>>, CancellationToken, Task<int>> _func = func;

        public Task<int> Handle(TestRequest request, Func<Task<int>> next, CancellationToken cancellationToken)
            => _func(request, next, cancellationToken);
    }

    private class IncrementBehavior : IPipelineBehavior<TestRequest, int>
    {
        public async Task<int> Handle(TestRequest request, Func<Task<int>> next, CancellationToken ct)
        {
            int result = await next();
            return result + 1;
        }
    }

    [Fact]
    public async Task Mediator_Should_Invoke_Handler_And_Behaviors()
    {
        ServiceCollection services = new();
        services.AddSingleton<IHandler<TestRequest, int>, TestHandler>();
        services.AddTransient<IPipelineBehavior<TestRequest, int>, IncrementBehavior>();
        ServiceProvider provider = services.BuildServiceProvider();

        Mediator mediator = new(provider);
        TestRequest request = new(Value: 5);

        int result = await mediator.Send(request);

        Assert.Equal(11, result);
    }

    [Fact]
    public async Task Mediator_Should_Propagate_Exception_From_Handler()
    {
        ServiceCollection services = new();
        services.AddSingleton<IHandler<TestRequest, int>, ExceptionHandler>();
        ServiceProvider provider = services.BuildServiceProvider();

        Mediator mediator = new(provider);
        TestRequest request = new(Value: 5);

        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(request));
    }

    [Fact]
    public async Task Mediator_Should_Execute_Behaviors_In_Order()
    {
        var order = new List<string>();
        var services = new ServiceCollection();
        services.AddSingleton<IHandler<TestRequest, int>, TestHandler>();
        services.AddTransient<IPipelineBehavior<TestRequest, int>>(sp =>
            new DelegateBehavior((req, next, ct) => { order.Add("First"); return next(); })
        );
        services.AddTransient<IPipelineBehavior<TestRequest, int>>(sp =>
            new DelegateBehavior((req, next, ct) => { order.Add("Second"); return next(); })
        );

        var provider = services.BuildServiceProvider();
        var mediator = new Mediator(provider);

        int result = await mediator.Send(new TestRequest(3));

        Assert.Equal(6, result);
        Assert.Equal(["First", "Second"], order);
    }
}
