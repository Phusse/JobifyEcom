using Jobify.Ecom.Application.CQRS.Messaging;

namespace Jobify.Ecom.Infrastructure.Tests.Utilities.Messaging;

public record TestRequest : IRequest<string>;
