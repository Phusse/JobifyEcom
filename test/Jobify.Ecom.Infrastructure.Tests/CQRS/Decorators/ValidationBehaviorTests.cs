using Jobify.Ecom.Infrastructure.CQRS.Decorators;
using Jobify.Ecom.Application.CQRS.Messaging;
using FluentValidation;
using Jobify.Ecom.Application.Exceptions;

namespace Jobify.Ecom.Infrastructure.Tests.CQRS.Decorators;

public class ValidationBehaviorTests
{
    private sealed record TestRequest(string Name) : IRequest<string>;

    private class PassValidator : AbstractValidator<TestRequest>
    {
        public PassValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private class FailValidator : AbstractValidator<TestRequest>
    {
        public FailValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    [Fact]
    public async Task Handle_Should_Call_Next_When_Validation_Passes()
    {
        var validator = new PassValidator();
        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator });
        var request = new TestRequest("John");

        var nextCalled = false;
        Task<string> Next() { nextCalled = true; return Task.FromResult("OK"); }

        var result = await behavior.Handle(request, Next, CancellationToken.None);

        Assert.True(nextCalled);
        Assert.Equal("OK", result);
    }

    [Fact]
    public async Task Handle_Should_Throw_AppException_When_Validation_Fails()
    {
        var validator = new FailValidator();
        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator });
        var request = new TestRequest(""); // invalid

        static Task<string> Next() => Task.FromResult("OK");

        var ex = await Assert.ThrowsAsync<AppException>(() =>
            behavior.Handle(request, Next, CancellationToken.None));

        Assert.Equal("SYSTEM_VALIDATION_FAILED", ex.Id);
        Assert.Contains("Name", ex.Details.First().Message);
    }
}
