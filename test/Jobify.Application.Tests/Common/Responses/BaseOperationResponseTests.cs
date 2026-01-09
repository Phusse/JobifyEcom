using FluentAssertions;
using Jobify.Application.Common.Responses;
using Jobify.Application.Enums;
using Jobify.Application.Models;

namespace Jobify.Application.Tests.Common.Responses;

public class BaseOperationResponseTests
{
    [Fact]
    public void WithTitle_ShouldSetTitle()
    {
        OperationOutcomeResponse response = new(Id: "1", Title: "Old", Details: []);

        OperationOutcomeResponse updated = response.WithTitle("New");

        updated.Id.Should().Be("1");
        updated.Title.Should().Be("New");
    }

    [Theory]
    [InlineData("Base", "Suffix", " ", "Base Suffix")]
    [InlineData("Base", "Suffix", "-", "Base-Suffix")]
    [InlineData("Base", "Suffix", "", "BaseSuffix")]
    [InlineData("Title", "Added", " : ", "Title : Added")]
    public void AppendTitle_ShouldAppendSuffix_WithSeparator(string baseTitle, string suffix, string separator, string expected)
    {
        OperationOutcomeResponse response = new(Id: "1", Title: baseTitle, Details: []);

        OperationOutcomeResponse updated = response.AppendTitle(suffix, separator);

        updated.Title.Should().Be(expected);
    }

    [Fact]
    public void WithDetails_ShouldReplaceDetails()
    {
        ResponseDetail[] details = [new ResponseDetail("Code", ResponseSeverity.Info)];
        OperationOutcomeResponse response = new(Id: "1", Title: "Title", Details: []);

        OperationOutcomeResponse updated = response.WithDetails(details);

        updated.Details.Should().BeEquivalentTo(details);
    }

    [Fact]
    public void AppendDetails_ShouldAppendNewDetails()
    {
        ResponseDetail[] initial = [new ResponseDetail("A", ResponseSeverity.Info)];
        ResponseDetail[] extra = [new ResponseDetail("B", ResponseSeverity.Info)];
        OperationOutcomeResponse response = new(Id: "1", Title: "Title", Details: initial);

        OperationOutcomeResponse updated = response.AppendDetails(extra);

        updated.Details.Should().HaveCount(2);
    }
}
