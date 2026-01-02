using Jobify.Ecom.Application.CQRS.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Jobify.Ecom.Infrastructure.CQRS.Messaging;

internal sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public Task<TResult> Send<TMessage, TResult>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IRequest<TResult>
    {
        IHandler<TMessage, TResult> handler = serviceProvider.GetRequiredService<IHandler<TMessage, TResult>>();
        return handler.Handle(message, cancellationToken);
    }
}
