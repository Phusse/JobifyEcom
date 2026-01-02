namespace Jobify.Ecom.Application.CQRS.Messaging;

public interface IMediator
{
    Task<TResult> Send<TMessage, TResult>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IRequest<TResult>;
}
