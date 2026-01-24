using FluentAssertions;
using Jobify.Ecom.Domain.Entities.JobApplications;
using Jobify.Ecom.Domain.Enums;
using Xunit;

namespace Jobify.Ecom.Domain.Tests.Entities.JobApplications;

public class JobApplicationTests
{
    private readonly Guid _jobId = Guid.NewGuid();
    private readonly Guid _applicantUserId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_CreatesApplication()
    {
        // Arrange & Act
        var application = new JobApplication(_jobId, _applicantUserId);

        // Assert
        application.JobId.Should().Be(_jobId);
        application.ApplicantUserId.Should().Be(_applicantUserId);
        application.Status.Should().Be(JobApplicationStatus.Submitted);
        application.Id.Should().NotBeEmpty();
        application.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateStatus_WithUndefinedStatus_ThrowsArgumentException()
    {
        // Arrange
        var application = new JobApplication(_jobId, _applicantUserId);

        // Act
        var act = () => application.UpdateStatus((JobApplicationStatus)999);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid job application status*");
    }

    [Fact]
    public void UpdateStatus_SubmittedToReviewed_Succeeds()
    {
        // Arrange
        var application = new JobApplication(_jobId, _applicantUserId);

        // Act
        application.UpdateStatus(JobApplicationStatus.Reviewed);

        // Assert
        application.Status.Should().Be(JobApplicationStatus.Reviewed);
    }

    [Fact]
    public void UpdateStatus_SubmittedToRejected_Succeeds()
    {
        // Arrange
        var application = new JobApplication(_jobId, _applicantUserId);

        // Act
        application.UpdateStatus(JobApplicationStatus.Rejected);

        // Assert
        application.Status.Should().Be(JobApplicationStatus.Rejected);
    }

    [Fact]
    public void UpdateStatus_SubmittedToShortlisted_Fails()
    {
        // Arrange
        var application = new JobApplication(_jobId, _applicantUserId);

        // Act
        var act = () => application.UpdateStatus(JobApplicationStatus.Shortlisted);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot change status from Submitted to Shortlisted*");
    }

    [Fact]
    public void UpdateStatus_ReviewedToShortlisted_Succeeds()
    {
        // Arrange
        var application = new JobApplication(_jobId, _applicantUserId);
        application.UpdateStatus(JobApplicationStatus.Reviewed);

        // Act
        application.UpdateStatus(JobApplicationStatus.Shortlisted);

        // Assert
        application.Status.Should().Be(JobApplicationStatus.Shortlisted);
    }

    [Fact]
    public void UpdateStatus_ReviewedToRejected_Succeeds()
    {
        // Arrange
        var application = new JobApplication(_jobId, _applicantUserId);
        application.UpdateStatus(JobApplicationStatus.Reviewed);

        // Act
        application.UpdateStatus(JobApplicationStatus.Rejected);

        // Assert
        application.Status.Should().Be(JobApplicationStatus.Rejected);
    }

    [Fact]
    public void UpdateStatus_ShortlistedToAccepted_Succeeds()
    {
        // Arrange
        var application = new JobApplication(_jobId, _applicantUserId);
        application.UpdateStatus(JobApplicationStatus.Reviewed);
        application.UpdateStatus(JobApplicationStatus.Shortlisted);

        // Act
        application.UpdateStatus(JobApplicationStatus.Accepted);

        // Assert
        application.Status.Should().Be(JobApplicationStatus.Accepted);
    }

    [Fact]
    public void UpdateStatus_ShortlistedToRejected_Succeeds()
    {
        // Arrange
        var application = new JobApplication(_jobId, _applicantUserId);
        application.UpdateStatus(JobApplicationStatus.Reviewed);
        application.UpdateStatus(JobApplicationStatus.Shortlisted);

        // Act
        application.UpdateStatus(JobApplicationStatus.Rejected);

        // Assert
        application.Status.Should().Be(JobApplicationStatus.Rejected);
    }

    [Fact]
    public void UpdateStatus_AcceptedToAny_ThrowsInvalidOperationException()
    {
        // Arrange
        var application = new JobApplication(_jobId, _applicantUserId);
        application.UpdateStatus(JobApplicationStatus.Reviewed);
        application.UpdateStatus(JobApplicationStatus.Shortlisted);
        application.UpdateStatus(JobApplicationStatus.Accepted);

        // Act
        var act = () => application.UpdateStatus(JobApplicationStatus.Rejected);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot change status from Accepted*");
    }

    [Fact]
    public void UpdateStatus_RejectedToAny_ThrowsInvalidOperationException()
    {
        // Arrange
        var application = new JobApplication(_jobId, _applicantUserId);
        application.UpdateStatus(JobApplicationStatus.Rejected);

        // Act
        var act = () => application.UpdateStatus(JobApplicationStatus.Reviewed);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot change status from Rejected*");
    }

    [Theory]
    [InlineData(JobApplicationStatus.Submitted, JobApplicationStatus.Reviewed, true)]
    [InlineData(JobApplicationStatus.Submitted, JobApplicationStatus.Rejected, true)]
    [InlineData(JobApplicationStatus.Submitted, JobApplicationStatus.Shortlisted, false)]
    [InlineData(JobApplicationStatus.Submitted, JobApplicationStatus.Accepted, false)]
    [InlineData(JobApplicationStatus.Reviewed, JobApplicationStatus.Shortlisted, true)]
    [InlineData(JobApplicationStatus.Reviewed, JobApplicationStatus.Rejected, true)]
    [InlineData(JobApplicationStatus.Reviewed, JobApplicationStatus.Accepted, false)]
    [InlineData(JobApplicationStatus.Shortlisted, JobApplicationStatus.Accepted, true)]
    [InlineData(JobApplicationStatus.Shortlisted, JobApplicationStatus.Rejected, true)]
    [InlineData(JobApplicationStatus.Accepted, JobApplicationStatus.Rejected, false)]
    [InlineData(JobApplicationStatus.Rejected, JobApplicationStatus.Accepted, false)]
    public void IsValidTransition_ReturnsCorrectResult(
        JobApplicationStatus current,
        JobApplicationStatus next,
        bool expected)
    {
        // Act
        var result = JobApplication.IsValidTransition(current, next);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void UpdateStatus_UpdatesAuditTimestamp()
    {
        // Arrange
        var application = new JobApplication(_jobId, _applicantUserId);
        var originalUpdatedAt = application.UpdatedAt;
        Thread.Sleep(10);

        // Act
        application.UpdateStatus(JobApplicationStatus.Reviewed);

        // Assert
        application.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}
