using FluentAssertions;
using Jobify.Ecom.Application.Features.Jobs.CreateJob;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Domain.Enums;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jobify.Ecom.Application.Tests.Features.Jobs.CreateJob;

public class CreateJobCommandHandlerTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly CreateJobCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();
    private const string ValidTitle = "Software Engineer";
    private const string ValidDescription = "We are looking for a skilled software engineer.";
    private const JobType ValidJobType = JobType.FullTime;
    private const decimal ValidMinSalary = 50000m;
    private const decimal ValidMaxSalary = 100000m;
    private readonly DateTime _validClosingDate = DateTime.UtcNow.AddDays(30);

    public CreateJobCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _handler = new CreateJobCommandHandler(_dbContext);
    }

    private CreateJobCommand CreateValidCommand(Guid? userId = null) => new(
        ValidTitle,
        ValidDescription,
        ValidJobType,
        ValidMinSalary,
        ValidMaxSalary,
        _validClosingDate,
        userId ?? _userId
    );

    [Fact]
    public async Task Handle_WithValidCommand_CreatesJob()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();

        var createdJob = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == result.Data);
        createdJob.Should().NotBeNull();
        createdJob!.Title.Should().Be(ValidTitle);
        createdJob.Description.Should().Be(ValidDescription);
        createdJob.JobType.Should().Be(ValidJobType);
        createdJob.MinSalary.Should().Be(ValidMinSalary);
        createdJob.MaxSalary.Should().Be(ValidMaxSalary);
    }

    [Fact]
    public async Task Handle_WithNullUserId_ThrowsException()
    {
        // Arrange
        var command = new CreateJobCommand(
            ValidTitle,
            ValidDescription,
            ValidJobType,
            ValidMinSalary,
            ValidMaxSalary,
            _validClosingDate,
            PostedByUserId: null
        );

        // Act
        var act = async () => await _handler.Handle(command);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Handle_WithValidData_SetsPostedByUserId()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command);

        // Assert
        var createdJob = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == result.Data);
        createdJob.Should().NotBeNull();
        createdJob!.PostedByUserId.Should().Be(_userId);
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsOperationResultWithJobId()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
        result.MessageId.Should().NotBeNullOrEmpty();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_CreatesMultipleJobs_EachHasUniqueId()
    {
        // Arrange
        var command1 = CreateValidCommand();
        var command2 = CreateValidCommand();

        // Act
        var result1 = await _handler.Handle(command1);
        var result2 = await _handler.Handle(command2);

        // Assert
        result1.Data.Should().NotBe(result2.Data);
        var jobCount = await _dbContext.Jobs.CountAsync();
        jobCount.Should().Be(2);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
