using Jobify.Application.CQRS.Messaging;

namespace Jobify.Infrastructure.Tests.Utilities.Messaging;

internal record TestRequest : IRequest<string>;
