using FluentAssertions;
using Jobify.Ecom.Application.Common.Responses;
using Jobify.Ecom.Application.Enums;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Tests.Common.Responses;

public class OperationOutcomeResponseTests
{
    [Fact]
    public void Constructor_ShouldAssignAllValuesCorrectly()
    {
        string id = "1";
        string title = "Title";
        ResponseDetail[] details = [new ResponseDetail("code", ResponseSeverity.Info)];

        OperationOutcomeResponse response = new(
            Id: id,
            Title: title,
            Details: details
        );

        response.Id.Should().Be(id);
        response.Title.Should().Be(title);
        response.Details.Should().BeSameAs(details);
        response.Data.Should().BeNull();
    }

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
