using FluentAssertions;
using Jobify.Application.Common.Responses;
using Jobify.Application.Enums;
using Jobify.Application.Models;

namespace Jobify.Application.Tests.Common.Responses;

public class OperationFailureResponseTests
{
    [Fact]
    public void Constructor_ShouldAssignAllValuesCorrectly()
    {
        string id = "failure-1";
        int statusCode = 400;
        string title = "Validation Failed";
        ResponseDetail[] details = [new ResponseDetail("code", ResponseSeverity.Info)];

        OperationFailureResponse response = new(
            Id: id,
            StatusCode: statusCode,
            Title: title,
            Details: details
        );

        response.Id.Should().Be(id);
        response.StatusCode.Should().Be(statusCode);
        response.Title.Should().Be(title);
        response.Details.Should().BeSameAs(details);
    }

    [Fact]
    public void WithTitle_ShouldReturnNewFailureResponse_WithUpdatedTitle()
    {
        OperationFailureResponse response = new(
            Id: "1",
            StatusCode: 404,
            Title: "Not Found",
            Details: []
        );

        OperationFailureResponse updated = response.WithTitle("Resource Missing");

        updated.Title.Should().Be("Resource Missing");
        updated.StatusCode.Should().Be(response.StatusCode);
        updated.Id.Should().Be(response.Id);
        updated.Should().NotBeSameAs(response);
    }


    [Fact]
    public void AppendDetails_ShouldAddNewDetails()
    {
        ResponseDetail[] initial = [new ResponseDetail("A", ResponseSeverity.Info)];
        ResponseDetail[] additional = [new ResponseDetail("B", ResponseSeverity.Info)];

        OperationFailureResponse response = new(
            Id: "1",
            StatusCode: 400,
            Title: "Bad Request",
            Details: initial
        );

        OperationFailureResponse updated = response.AppendDetails(additional);

        updated.Details.Should().HaveCount(2);
        updated.Details.Should().Contain(initial[0]);
        updated.Details.Should().Contain(additional[0]);
    }
}
