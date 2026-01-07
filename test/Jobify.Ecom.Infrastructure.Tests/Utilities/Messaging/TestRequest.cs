using Jobify.Ecom.Application.CQRS.Messaging;

namespace Jobify.Ecom.Infrastructure.Tests.Utilities.Messaging;

internal record TestRequest : IRequest<string>;
