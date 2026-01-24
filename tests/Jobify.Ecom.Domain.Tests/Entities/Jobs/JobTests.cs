using FluentAssertions;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Domain.Enums;
using Xunit;

namespace Jobify.Ecom.Domain.Tests.Entities.Jobs;

public class JobTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private const string ValidTitle = "Software Engineer";
    private const string ValidDescription = "We are looking for a skilled software engineer.";
    private const JobType ValidJobType = JobType.FullTime;
    private const decimal ValidMinSalary = 50000m;
    private const decimal ValidMaxSalary = 100000m;
    private readonly DateTime _validClosingDate = DateTime.UtcNow.AddDays(30);

    private Job CreateValidJob() => new(
        _userId, ValidTitle, ValidDescription, ValidJobType,
        ValidMinSalary, ValidMaxSalary, _validClosingDate);

    [Fact]
    public void Constructor_WithValidData_CreatesJob()
    {
        // Arrange & Act
        var job = CreateValidJob();

        // Assert
        job.PostedByUserId.Should().Be(_userId);
        job.Title.Should().Be(ValidTitle);
        job.Description.Should().Be(ValidDescription);
        job.JobType.Should().Be(ValidJobType);
        job.MinSalary.Should().Be(ValidMinSalary);
        job.MaxSalary.Should().Be(ValidMaxSalary);
        job.ClosingDate.Should().BeCloseTo(_validClosingDate, TimeSpan.FromSeconds(1));
        job.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateTitle_WithNullOrEmpty_ThrowsArgumentException(string? title)
    {
        // Arrange
        var job = CreateValidJob();

        // Act
        var act = () => job.UpdateTitle(title!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*title is required*");
    }

    [Fact]
    public void UpdateTitle_WhenTooLong_ThrowsArgumentException()
    {
        // Arrange
        var job = CreateValidJob();
        var longTitle = new string('a', JobLimits.TitleMaxLength + 1);

        // Act
        var act = () => job.UpdateTitle(longTitle);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"*between {JobLimits.TitleMinLength} and {JobLimits.TitleMaxLength}*");
    }

    [Fact]
    public void UpdateTitle_WithValidTitle_UpdatesTitleAndAudit()
    {
        // Arrange
        var job = CreateValidJob();
        var originalUpdatedAt = job.UpdatedAt;
        var newTitle = "Senior Software Engineer";
        Thread.Sleep(10);

        // Act
        job.UpdateTitle(newTitle);

        // Assert
        job.Title.Should().Be(newTitle);
        job.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDescription_WithNullOrEmpty_ThrowsArgumentException(string? description)
    {
        // Arrange
        var job = CreateValidJob();

        // Act
        var act = () => job.UpdateDescription(description!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*description is required*");
    }

    [Fact]
    public void UpdateDescription_WhenTooLong_ThrowsArgumentException()
    {
        // Arrange
        var job = CreateValidJob();
        var longDescription = new string('a', JobLimits.DescriptionMaxLength + 1);

        // Act
        var act = () => job.UpdateDescription(longDescription);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"*between {JobLimits.DescriptionMinLength} and {JobLimits.DescriptionMaxLength}*");
    }

    [Fact]
    public void UpdateJobType_WithUndefinedType_ThrowsArgumentException()
    {
        // Arrange
        var job = CreateValidJob();

        // Act
        var act = () => job.UpdateJobType((JobType)999);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid job type*");
    }

    [Theory]
    [InlineData(JobType.FullTime)]
    [InlineData(JobType.PartTime)]
    [InlineData(JobType.Contract)]
    [InlineData(JobType.Internship)]
    [InlineData(JobType.Remote)]
    public void UpdateJobType_WithValidType_UpdatesJobType(JobType jobType)
    {
        // Arrange
        var job = CreateValidJob();

        // Act
        job.UpdateJobType(jobType);

        // Assert
        job.JobType.Should().Be(jobType);
    }

    [Fact]
    public void UpdateSalary_WithMinBelowLimit_ThrowsArgumentException()
    {
        // Arrange
        var job = CreateValidJob();

        // Act
        var act = () => job.UpdateSalary(-1, ValidMaxSalary);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Salary range is invalid*");
    }

    [Fact]
    public void UpdateSalary_WithMaxAboveLimit_ThrowsArgumentException()
    {
        // Arrange
        var job = CreateValidJob();

        // Act
        var act = () => job.UpdateSalary(ValidMinSalary, JobLimits.MaxAllowedMoney + 1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Salary range is invalid*");
    }

    [Fact]
    public void UpdateSalary_WithMaxBelowMin_ThrowsArgumentException()
    {
        // Arrange
        var job = CreateValidJob();

        // Act
        var act = () => job.UpdateSalary(100000, 50000);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Salary range is invalid*");
    }

    [Fact]
    public void UpdateSalary_WithValidRange_UpdatesSalary()
    {
        // Arrange
        var job = CreateValidJob();

        // Act
        job.UpdateSalary(60000, 120000);

        // Assert
        job.MinSalary.Should().Be(60000);
        job.MaxSalary.Should().Be(120000);
    }

    [Fact]
    public void UpdateClosingDate_WithPastDate_ThrowsArgumentException()
    {
        // Arrange
        var job = CreateValidJob();

        // Act
        var act = () => job.UpdateClosingDate(DateTime.UtcNow.AddDays(-1));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Closing date must be in the future*");
    }

    [Fact]
    public void UpdateClosingDate_WithFutureDate_UpdatesClosingDate()
    {
        // Arrange
        var job = CreateValidJob();
        var newClosingDate = DateTime.UtcNow.AddDays(60);

        // Act
        job.UpdateClosingDate(newClosingDate);

        // Assert
        job.ClosingDate.Should().BeCloseTo(newClosingDate, TimeSpan.FromSeconds(1));
    }
}
