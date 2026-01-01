using FluentAssertions;
using Jobify.Ecom.Application.Common.Responses;
using Jobify.Ecom.Application.Enums;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Tests.Extensions.Responses;

public class OperationOutcomeResponseExtensionsTests
{
    [Fact]
    public void ToOperationResult_Generic_ShouldMapAllValuesCorrectly()
    {
        ResponseDetail[] details = [new ResponseDetail("code", ResponseSeverity.Info)];

        OperationOutcomeResponse<int> response = new(
            Id: "MSG_001",
            Title: "Success",
            Details: details,
            Data: 42
        );

        OperationResult<int> result = response.ToOperationResult();

        result.MessageId.Should().Be("MSG_001");
        result.Message.Should().Be("Success");
        result.Data.Should().Be(42);
        result.Details.Should().BeEquivalentTo(details);
        result.Details.Should().NotBeSameAs(details);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("   ", "   ")]
    public void ToOperationResult_Generic_ShouldNormalizeIdAndMessage(string? id, string? title)
    {
        OperationOutcomeResponse<string> response = new(
            Id: id!,
            Title: title!,
            Details: [],
            Data: "data"
        );

        OperationResult<string> result = response.ToOperationResult();

        result.MessageId.Should().Be("UNKNOWN_ID");
        result.Message.Should().Be("No message provided.");
    }

    [Fact]
    public void ToOperationResult_Generic_ShouldSetDetailsToNull_WhenEmpty()
    {
        OperationOutcomeResponse<string> response = new(
            Id: "ID",
            Title: "Title",
            Details: [],
            Data: "data"
        );

        OperationResult<string> result = response.ToOperationResult();

        result.Details.Should().BeNull();
    }

    [Fact]
    public void ToOperationResult_NonGeneric_ShouldMapAllValuesCorrectly()
    {
        OperationOutcomeResponse response = new(
            Id: "MSG_002",
            Title: "Completed",
            Details: [],
            Data: new { Value = 10 }
        );

        OperationResult<object?> result = response.ToOperationResult();

        result.MessageId.Should().Be("MSG_002");
        result.Message.Should().Be("Completed");
        result.Data.ShouldBeEquivalentTo(response.Data);
    }
}
