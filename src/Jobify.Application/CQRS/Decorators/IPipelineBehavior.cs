using Jobify.Application.CQRS.Messaging;

namespace Jobify.Application.CQRS.Decorators;

public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage<TResponse>
{
    Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default);
}
