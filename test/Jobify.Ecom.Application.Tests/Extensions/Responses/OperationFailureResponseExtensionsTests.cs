using FluentAssertions;
using Jobify.Ecom.Application.Common.Responses;
using Jobify.Ecom.Application.Exceptions;
using Jobify.Ecom.Application.Extensions.Responses;

namespace Jobify.Ecom.Application.Tests.Extensions.Responses;

public class OperationFailureResponseExtensionsTests
{
    [Fact]
    public void ToException_ShouldCreateAppException_FromFailureResponse()
    {
        OperationFailureResponse failure = new(
            Id: "ERR_001",
            StatusCode: 400,
            Title: "Bad Request",
            Details: []
        );

        AppException exception = failure.ToException();

        exception.Should().NotBeNull();
        exception.Id.Should().Be("ERR_001");
        exception.StatusCode.Should().Be(400);
        exception.Message.Should().Be("Bad Request");
    }
}
