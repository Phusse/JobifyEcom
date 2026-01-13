namespace Jobify.Application.CQRS.Messaging;

public interface IHandler<in TMessage, TResult> where TMessage : IRequest<TResult>
{
    Task<TResult> Handle(TMessage message, CancellationToken cancellationToken = default);
}
