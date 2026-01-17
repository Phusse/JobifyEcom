namespace Jobify.Application.CQRS.Messaging;

public interface IHandler<in TMessage, TResult>
    where TMessage : IMessage<TResult>
{
    Task<TResult> Handle(TMessage message, CancellationToken cancellationToken = default);
}
