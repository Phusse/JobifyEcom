using FluentAssertions;
using Jobify.Ecom.Application.Common.Responses;

namespace Jobify.Ecom.Application.Tests.Common.Responses;

public class OperationOutcomeResponseTests
{
    [Fact]
    public void WithData_ShouldSetData()
    {
        OperationOutcomeResponse<string> response = new(Id: "1", Title: "Title", Details: []);

        OperationOutcomeResponse<string> updated = response.WithData("Hello");

        updated.Data.Should().Be("Hello");
    }

    [Fact]
    public void As_ShouldCreateTypedResponse()
    {
        OperationOutcomeResponse response = new(Id: "1", Title: "Title", Details: []);

        OperationOutcomeResponse<string> typed = response.As<string>();

        typed.Id.Should().Be(response.Id);
        typed.Title.Should().Be(response.Title);
        typed.Details.Should().BeEquivalentTo(response.Details);
    }
}
