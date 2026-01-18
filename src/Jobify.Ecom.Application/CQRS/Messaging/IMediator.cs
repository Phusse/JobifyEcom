namespace Jobify.Ecom.Application.CQRS.Messaging;

public interface IMediator
{
    Task<TResult> Send<TResult>(IMessage<TResult> message, CancellationToken cancellationToken = default);
}
