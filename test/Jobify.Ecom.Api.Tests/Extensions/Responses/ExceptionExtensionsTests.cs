using FluentAssertions;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.Enums;
using Jobify.Ecom.Application.Exceptions;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Api.Tests.Extensions.Responses;

public class ExceptionExtensionsTests
{
    [Fact]
    public void ToApiResponse_WithAppException_ShouldReturnCorrectResponse()
    {
        ResponseDetail[] details = [new("Detail message", ResponseSeverity.Error)];
        AppException exception = new(
            id: "Custom Error",
            statusCode: 100,
            title: "Custom Message",
            details: details
        );

        ApiResponse<object> response = exception.ToApiResponse<object>();

        response.Success.Should().BeFalse();
        response.MessageId.Should().Be(exception.Id);
        response.Message.Should().Be(exception.Message);
        response.Details.Should().BeEquivalentTo(details);
        response.Data.Should().BeNull();
    }

    [Fact]
    public void ToApiResponse_WithGenericException_ShouldReturnUnexpectedErrorResponse()
    {
        Exception exception = new("System error");

        ApiResponse<object> response = exception.ToApiResponse<object>();

        response.Success.Should().BeFalse();
        response.MessageId.Should().Be("UNEXPECTED_ERROR");
        response.Message.Should().Be("Something went wrong on our side. Please try again later.");
        response.Details.Should().ContainSingle(d => d.Message.Contains("contact support"));
        response.Data.Should().BeNull();
    }
}
