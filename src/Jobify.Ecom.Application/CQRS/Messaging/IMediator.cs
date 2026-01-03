namespace Jobify.Ecom.Application.CQRS.Messaging;

public interface IMediator
{
    Task<TResult> Send<TResult>(IRequest<TResult> message, CancellationToken cancellationToken = default);
}
