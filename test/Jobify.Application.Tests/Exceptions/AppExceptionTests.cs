using FluentAssertions;
using Jobify.Application.Common.Responses;
using Jobify.Application.Enums;
using Jobify.Application.Exceptions;
using Jobify.Application.Models;

namespace Jobify.Application.Tests.Exceptions;

public class AppExceptionTests
{
    [Fact]
    public void Constructor_FromOperationFailureResponse_ShouldMapAllValuesCorrectly()
    {
        ResponseDetail[] details = [new ResponseDetail("code", ResponseSeverity.Info)];

        OperationFailureResponse failure = new(
            Id: "ERR_001",
            StatusCode: 400,
            Title: "Bad Request",
            Details: details
        );

        AppException exception = new(failure);

        exception.Id.Should().Be("ERR_001");
        exception.StatusCode.Should().Be(400);
        exception.Message.Should().Be("Bad Request");
        exception.Details.Should().BeEquivalentTo(details);
        exception.Details.Should().NotBeSameAs(details);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_FromFailure_ShouldFallbackToUnknownErrorId(string? id)
    {
        OperationFailureResponse failure = new(
            Id: id!,
            StatusCode: 400,
            Title: "Error",
            Details: []
        );

        AppException exception = new(failure);

        exception.Id.Should().Be("UNKNOWN_ERROR");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(99)]
    [InlineData(600)]
    [InlineData(999)]
    public void Constructor_FromFailure_ShouldFallbackStatusCodeTo500(int statusCode)
    {
        OperationFailureResponse failure = new(
            Id: "ERR",
            StatusCode: statusCode,
            Title: "Error",
            Details: []
        );

        AppException exception = new(failure);

        exception.StatusCode.Should().Be(500);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_FromFailure_ShouldUseDefaultMessage_WhenTitleIsInvalid(string? title)
    {
        OperationFailureResponse failure = new(
            Id: "ERR",
            StatusCode: 400,
            Title: title!,
            Details: []
        );

        AppException exception = new(failure);

        exception.Message.Should().Be("An unexpected error occurred.");
    }

    [Fact]
    public void Constructor_FromFailure_ShouldSetDetailsToNull_WhenEmpty()
    {
        OperationFailureResponse failure = new(
            Id: "ERR",
            StatusCode: 400,
            Title: "Error",
            Details: []
        );

        AppException exception = new(failure);

        exception.Details.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithRawValues_ShouldAssignCorrectly()
    {
        ResponseDetail[] details = [new ResponseDetail("code", ResponseSeverity.Info)];

        AppException exception = new(
            id: "ERR_002",
            statusCode: 422,
            title: "Validation Error",
            details: details
        );

        exception.Id.Should().Be("ERR_002");
        exception.StatusCode.Should().Be(422);
        exception.Message.Should().Be("Validation Error");
        exception.Details.Should().BeEquivalentTo(details);
        exception.Details.Should().NotBeSameAs(details);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithRawValues_ShouldFallbackMessage(string? title)
    {
        AppException exception = new(
            id: "ERR",
            statusCode: 400,
            title: title
        );

        exception.Message.Should().Be("An unexpected error occurred.");
    }
}
