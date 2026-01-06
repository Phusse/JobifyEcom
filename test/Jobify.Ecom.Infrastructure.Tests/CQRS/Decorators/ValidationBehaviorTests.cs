using Jobify.Ecom.Infrastructure.CQRS.Decorators;
using Jobify.Ecom.Application.CQRS.Messaging;
using FluentValidation;
using Jobify.Ecom.Application.Exceptions;

namespace Jobify.Ecom.Infrastructure.Tests.CQRS.Decorators;

public class ValidationBehaviorTests
{
    private record UserNameRequest(string Name) : IRequest<string>;

    private class UserNameValidator : AbstractValidator<UserNameRequest>
    {
        public UserNameValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    [Fact]
    public async Task Handle_Should_Call_Next_When_Validation_Passes()
    {
        UserNameValidator validator = new();
        ValidationBehavior<UserNameRequest, string> behavior = new([validator]);
        UserNameRequest request = new("John");

        bool nextCalled = false;
        Task<string> Next() { nextCalled = true; return Task.FromResult("OK"); }

        string result = await behavior.Handle(request, Next, CancellationToken.None);

        Assert.True(nextCalled);
        Assert.Equal("OK", result);
    }

    [Fact]
    public async Task Handle_Should_Throw_AppException_When_Validation_Fails()
    {
        UserNameValidator validator = new();
        ValidationBehavior<UserNameRequest, string> behavior = new([validator]);
        UserNameRequest request = new("");

        static Task<string> Next() => Task.FromResult("OK");

        AppException ex = await Assert.ThrowsAsync<AppException>(() =>
            behavior.Handle(request, Next, CancellationToken.None)
        );

        Assert.Equal("SYSTEM_VALIDATION_FAILED", ex.Id);
    }
}
