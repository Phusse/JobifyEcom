using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using JobifyEcom.Controllers;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Jobs;
using JobifyEcom.Services;
using JobifyEcom.Enums;

namespace JobifyEcom.Tests.Controllers;

[TestFixture]
public class JobControllerTests
{
    private Mock<IJobService> _jobServiceMock = null!;
    private Mock<IJobApplicationService> _jobApplicationServiceMock = null!;
    private JobController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _jobServiceMock = new Mock<IJobService>();
        _jobApplicationServiceMock = new Mock<IJobApplicationService>();
        _controller = new JobController(_jobServiceMock.Object, _jobApplicationServiceMock.Object);

        // Setup HttpContext for Request
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("api.test.com");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    private static JobResponse CreateValidJobResponse(Guid? id = null, string title = "Software Engineer") => new()
    {
        Id = id ?? Guid.NewGuid(),
        Title = title,
        Description = "Build stuff",
        Price = 500m,
        Status = JobStatus.Open,
        CreatedAt = DateTime.UtcNow,
        PostedByUserId = Guid.NewGuid()
    };

    private static JobApplicationResponse CreateValidJobApplicationResponse(Guid? id = null, Guid? jobId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Status = JobApplicationStatus.Pending,
        DateRequested = DateTime.UtcNow,
        WorkerId = Guid.NewGuid(),
        WorkerName = "Test Worker",
        JobPostId = jobId ?? Guid.NewGuid(),
        JobTitle = "Test Job",
        JobPrice = 100m
    };

    #region Create Job Tests

    [Test]
    public async Task Create_WithValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var request = new JobCreateRequest { Title = "Software Engineer", Description = "Build stuff" };
        var jobResponse = CreateValidJobResponse();
        var serviceResult = new ServiceResult<JobResponse>
        {
            Data = jobResponse,
            Message = "Job created"
        };

        _jobServiceMock.Setup(s => s.CreateJobAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedResult>());
        _jobServiceMock.Verify(s => s.CreateJobAsync(request), Times.Once);
    }

    [Test]
    public async Task Create_CallsJobService_WithCorrectRequest()
    {
        // Arrange
        var request = new JobCreateRequest { Title = "Test Job", Description = "Test Description" };
        var serviceResult = new ServiceResult<JobResponse> { Data = CreateValidJobResponse() };

        _jobServiceMock.Setup(s => s.CreateJobAsync(It.IsAny<JobCreateRequest>())).ReturnsAsync(serviceResult);

        // Act
        await _controller.Create(request);

        // Assert
        _jobServiceMock.Verify(s => s.CreateJobAsync(It.Is<JobCreateRequest>(r => r.Title == "Test Job")), Times.Once);
    }

    #endregion

    #region GetById Tests

    [Test]
    public async Task GetById_WithExistingJob_ReturnsOkResult()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobResponse = CreateValidJobResponse(jobId);
        var serviceResult = new ServiceResult<JobResponse?>
        {
            Data = jobResponse,
            Message = "Job found"
        };

        _jobServiceMock.Setup(s => s.GetJobByIdAsync(jobId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetById(jobId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _jobServiceMock.Verify(s => s.GetJobByIdAsync(jobId), Times.Once);
    }

    [Test]
    public async Task GetById_WithNonExistentJob_ReturnsOkWithNullData()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var serviceResult = new ServiceResult<JobResponse?>
        {
            Data = null,
            Message = "Job not found"
        };

        _jobServiceMock.Setup(s => s.GetJobByIdAsync(jobId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetById(jobId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    #endregion

    #region Update Tests

    [Test]
    public async Task Update_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var request = new JobUpdateRequest { Title = "Updated Title" };
        var jobResponse = CreateValidJobResponse(jobId, "Updated Title");
        var serviceResult = new ServiceResult<JobResponse>
        {
            Data = jobResponse,
            Message = "Job updated"
        };

        _jobServiceMock.Setup(s => s.UpdateJobAsync(jobId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Update(jobId, request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _jobServiceMock.Verify(s => s.UpdateJobAsync(jobId, request), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Test]
    public async Task Delete_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "Job deleted"
        };

        _jobServiceMock.Setup(s => s.DeleteJobAsync(jobId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Delete(jobId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _jobServiceMock.Verify(s => s.DeleteJobAsync(jobId), Times.Once);
    }

    #endregion

    #region Apply Tests

    [Test]
    public async Task Apply_WithValidJobId_ReturnsCreatedResult()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var applicationResponse = CreateValidJobApplicationResponse(null, jobId);
        var serviceResult = new ServiceResult<JobApplicationResponse>
        {
            Data = applicationResponse,
            Message = "Application submitted"
        };

        _jobApplicationServiceMock.Setup(s => s.CreateApplicationAsync(jobId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Apply(jobId);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedResult>());
        _jobApplicationServiceMock.Verify(s => s.CreateApplicationAsync(jobId), Times.Once);
    }

    #endregion

    #region GetApplicationById Tests

    [Test]
    public async Task GetApplicationById_WithValidIds_ReturnsOkResult()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var applicationResponse = CreateValidJobApplicationResponse(applicationId, jobId);
        var serviceResult = new ServiceResult<JobApplicationResponse>
        {
            Data = applicationResponse,
            Message = "Application found"
        };

        _jobApplicationServiceMock.Setup(s => s.GetByIdAsync(jobId, applicationId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetApplicationById(jobId, applicationId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _jobApplicationServiceMock.Verify(s => s.GetByIdAsync(jobId, applicationId), Times.Once);
    }

    #endregion

    #region AcceptApplication Tests

    [Test]
    public async Task AcceptApplication_WithValidIds_ReturnsOkResult()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "Application accepted"
        };

        _jobApplicationServiceMock.Setup(s => s.UpdateStatusAsync(jobId, applicationId, JobApplicationStatus.Accepted))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.AcceptApplication(jobId, applicationId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _jobApplicationServiceMock.Verify(s => s.UpdateStatusAsync(jobId, applicationId, JobApplicationStatus.Accepted), Times.Once);
    }

    #endregion

    #region RejectApplication Tests

    [Test]
    public async Task RejectApplication_WithValidIds_ReturnsOkResult()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "Application rejected"
        };

        _jobApplicationServiceMock.Setup(s => s.UpdateStatusAsync(jobId, applicationId, JobApplicationStatus.Rejected))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.RejectApplication(jobId, applicationId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _jobApplicationServiceMock.Verify(s => s.UpdateStatusAsync(jobId, applicationId, JobApplicationStatus.Rejected), Times.Once);
    }

    #endregion
}
