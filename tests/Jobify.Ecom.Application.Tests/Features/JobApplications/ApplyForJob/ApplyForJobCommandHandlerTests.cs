using FluentAssertions;
using Jobify.Ecom.Application.Features.JobApplications.ApplyForJob;
using Jobify.Ecom.Domain.Entities.JobApplications;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Domain.Enums;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jobify.Ecom.Application.Tests.Features.JobApplications.ApplyForJob;

public class ApplyForJobCommandHandlerTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly ApplyForJobCommandHandler _handler;

    private readonly Guid _jobPosterId = Guid.NewGuid();
    private readonly Guid _applicantUserId = Guid.NewGuid();

    public ApplyForJobCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _handler = new ApplyForJobCommandHandler(_dbContext);
    }

    private Job CreateTestJob(Guid? postedByUserId = null, DateTime? closingDate = null)
    {
        return new Job(
            postedByUserId ?? _jobPosterId,
            "Test Job",
            "Test Description for job posting",
            JobType.FullTime,
            50000m,
            100000m,
            closingDate ?? DateTime.UtcNow.AddDays(30)
        );
    }

    private async Task<Job> SeedJobAsync(Job? job = null)
    {
        job ??= CreateTestJob();
        _dbContext.Jobs.Add(job);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear(); // Clear tracker to avoid navigation conflicts
        return job;
    }

    [Fact]
    public async Task Handle_WithValidApplication_CreatesJobApplication()
    {
        // Arrange
        var job = await SeedJobAsync();
        var command = new ApplyForJobCommand(job.Id, _applicantUserId);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();

        var application = await _dbContext.JobApplications
            .FirstOrDefaultAsync(a => a.Id == result.Data);
        application.Should().NotBeNull();
        application!.JobId.Should().Be(job.Id);
        application.ApplicantUserId.Should().Be(_applicantUserId);
        application.Status.Should().Be(JobApplicationStatus.Submitted);
    }

    [Fact]
    public async Task Handle_WithNullApplicantUserId_ThrowsException()
    {
        // Arrange
        var job = await SeedJobAsync();
        var command = new ApplyForJobCommand(job.Id, ApplicantUserId: null);

        // Act
        var act = async () => await _handler.Handle(command);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Handle_WithNonExistentJob_ThrowsException()
    {
        // Arrange
        var command = new ApplyForJobCommand(Guid.NewGuid(), _applicantUserId);

        // Act
        var act = async () => await _handler.Handle(command);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Handle_WhenApplyingToOwnJob_ThrowsException()
    {
        // Arrange - job poster is the same as applicant
        var job = await SeedJobAsync(CreateTestJob(postedByUserId: _applicantUserId));
        var command = new ApplyForJobCommand(job.Id, _applicantUserId);

        // Act
        var act = async () => await _handler.Handle(command);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact(Skip = "Domain model prevents Job creation with past closing dates. This scenario requires integration testing with database manipulation.")]
    public async Task Handle_WhenJobIsClosed_ThrowsException()
    {
        // This test cannot be run as a unit test because the Job entity
        // validates closing dates during construction, preventing creation
        // of jobs with past dates. Testing this scenario requires:
        // 1. Integration tests with direct database access, OR
        // 2. Modifying the job's closing date post-creation (if allowed)
        
        // Arrange - job with past closing date
        // var closedJob = CreateTestJob(closingDate: DateTime.UtcNow.AddDays(-1));
        // await SeedJobAsync(closedJob);
        // var command = new ApplyForJobCommand(closedJob.Id, _applicantUserId);

        // Act
        // var act = async () => await _handler.Handle(command);

        // Assert
        // await act.Should().ThrowAsync<Exception>();
        await Task.CompletedTask;
    }

    [Fact]
    public async Task Handle_WhenAlreadyApplied_ThrowsException()
    {
        // Arrange
        var job = await SeedJobAsync();

        // First application
        var firstApplication = new JobApplication(job.Id, _applicantUserId);
        _dbContext.JobApplications.Add(firstApplication);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear(); // Clear tracker to avoid navigation conflicts

        // Try to apply again
        var command = new ApplyForJobCommand(job.Id, _applicantUserId);

        // Act
        var act = async () => await _handler.Handle(command);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Handle_MultipleUsersApplyToSameJob_AllSucceed()
    {
        // Arrange
        var job = await SeedJobAsync();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var command1 = new ApplyForJobCommand(job.Id, userId1);
        var command2 = new ApplyForJobCommand(job.Id, userId2);

        // Act
        var result1 = await _handler.Handle(command1);
        var result2 = await _handler.Handle(command2);

        // Assert
        result1.Data.Should().NotBeEmpty();
        result2.Data.Should().NotBeEmpty();
        result1.Data.Should().NotBe(result2.Data);

        var applicationCount = await _dbContext.JobApplications
            .CountAsync(a => a.JobId == job.Id);
        applicationCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_SameUserAppliestoMultipleJobs_AllSucceed()
    {
        // Arrange
        var job1 = await SeedJobAsync();
        var job2 = await SeedJobAsync(CreateTestJob());

        var command1 = new ApplyForJobCommand(job1.Id, _applicantUserId);
        var command2 = new ApplyForJobCommand(job2.Id, _applicantUserId);

        // Act
        var result1 = await _handler.Handle(command1);
        var result2 = await _handler.Handle(command2);

        // Assert
        result1.Data.Should().NotBeEmpty();
        result2.Data.Should().NotBeEmpty();

        var applicationCount = await _dbContext.JobApplications
            .CountAsync(a => a.ApplicantUserId == _applicantUserId);
        applicationCount.Should().Be(2);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
