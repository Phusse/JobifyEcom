using FluentAssertions;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.Enums;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Api.Tests.Extensions.Responses;

public class OperationResultExtensionTests
{
    [Fact]
    public void ToApiResponse_ShouldMapPropertiesCorrectly()
    {
        List<ResponseDetail> details = [new("Detail message", ResponseSeverity.Info)];

        OperationResult<string> result = new(
            MessageId: "SUCCESS_ID",
            Message: "Operation successful",
            Details: details,
            Data: "Test Data"
        );

        ApiResponse<string> response = result.ToApiResponse();

        response.Success.Should().BeTrue();
        response.MessageId.Should().Be("SUCCESS_ID");
        response.Message.Should().Be("Operation successful");
        response.Details.Should().BeEquivalentTo(details);
        response.Data.Should().Be("Test Data");
    }

    [Fact]
    public void WithoutData_ShouldRemoveDataButKeepOtherProperties()
    {
        List<ResponseDetail> details = [new("Detail message", ResponseSeverity.Info)];
        OperationResult<string> result = new(
            MessageId: "SUCCESS_ID",
            Message: "Operation successful",
            Details: details,
            Data: "Test Data"
        );

        OperationResult<object> newResult = result.WithoutData();

        newResult.MessageId.Should().Be("SUCCESS_ID");
        newResult.Message.Should().Be("Operation successful");
        newResult.Details.Should().BeEquivalentTo(details);
        newResult.Data.Should().BeNull();
    }
}
