using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using JobifyEcom.Controllers;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;
using JobifyEcom.Services;
using JobifyEcom.Enums;

namespace JobifyEcom.Tests.Controllers;

[TestFixture]
public class WorkerControllerTests
{
    private Mock<IWorkerService> _workerServiceMock = null!;
    private Mock<IWorkerSkillService> _workerSkillServiceMock = null!;
    private WorkerController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _workerServiceMock = new Mock<IWorkerService>();
        _workerSkillServiceMock = new Mock<IWorkerSkillService>();
        _controller = new WorkerController(_workerServiceMock.Object, _workerSkillServiceMock.Object);

        // Setup HttpContext for Request
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("api.test.com");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    private static WorkerProfileResponse CreateValidWorkerProfileResponse(Guid? workerId = null) => new()
    {
        WorkerId = workerId ?? Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow
    };

    private static WorkerSkillResponse CreateValidWorkerSkillResponse(
        Guid? id = null, 
        string name = "C# Programming",
        SkillLevel level = SkillLevel.Expert) => new()
    {
        Id = id ?? Guid.NewGuid(),
        WorkerId = Guid.NewGuid(),
        Name = name,
        Description = "A skill description",
        Level = level,
        YearsOfExperience = 5,
        CertificationLink = null,
        Tags = ["Backend", "C#"],
        VerificationStatus = VerificationStatus.Pending
    };

    #region CreateProfile Tests

    [Test]
    public async Task CreateProfile_ReturnsCreatedResult()
    {
        // Arrange
        var serviceResult = new ServiceResult<object>
        {
            Data = new { Message = "Profile created" },
            Message = "Worker profile created successfully"
        };

        _workerServiceMock.Setup(s => s.CreateProfileAsync()).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.CreateProfile();

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedResult>());
        _workerServiceMock.Verify(s => s.CreateProfileAsync(), Times.Once);
    }

    #endregion

    #region GetMyProfile Tests

    [Test]
    public async Task GetMyProfile_WithExistingProfile_ReturnsOkResult()
    {
        // Arrange
        var workerProfile = CreateValidWorkerProfileResponse();
        var serviceResult = new ServiceResult<WorkerProfileResponse>
        {
            Data = workerProfile,
            Message = "Profile retrieved"
        };

        _workerServiceMock.Setup(s => s.GetMyProfileAsync()).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetMyProfile();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _workerServiceMock.Verify(s => s.GetMyProfileAsync(), Times.Once);
    }

    #endregion

    #region DeleteProfile Tests

    [Test]
    public async Task DeleteProfile_ReturnsOkResult()
    {
        // Arrange
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "Profile deleted"
        };

        _workerServiceMock.Setup(s => s.DeleteProfileAsync()).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.DeleteProfile();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _workerServiceMock.Verify(s => s.DeleteProfileAsync(), Times.Once);
    }

    #endregion

    #region AddSkill Tests

    [Test]
    public async Task AddSkill_WithValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var request = new AddWorkerSkillRequest 
        { 
            Name = "C# Programming", 
            Level = SkillLevel.Expert,
            YearsOfExperience = 5,
            Tags = ["Backend", "C#"]
        };
        var skillResponse = CreateValidWorkerSkillResponse();
        var serviceResult = new ServiceResult<WorkerSkillResponse>
        {
            Data = skillResponse,
            Message = "Skill added"
        };

        _workerSkillServiceMock.Setup(s => s.AddSkillAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.AddSkill(request);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedResult>());
        _workerSkillServiceMock.Verify(s => s.AddSkillAsync(request), Times.Once);
    }

    #endregion

    #region RemoveSkill Tests

    [Test]
    public async Task RemoveSkill_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "Skill removed"
        };

        _workerSkillServiceMock.Setup(s => s.RemoveSkillAsync(skillId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.RemoveSkill(skillId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _workerSkillServiceMock.Verify(s => s.RemoveSkillAsync(skillId), Times.Once);
    }

    #endregion

    #region GetSkillById Tests

    [Test]
    public async Task GetSkillById_WithExistingSkill_ReturnsOkResult()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var skillResponse = CreateValidWorkerSkillResponse(skillId, "JavaScript", SkillLevel.Intermediate);
        var serviceResult = new ServiceResult<WorkerSkillResponse>
        {
            Data = skillResponse,
            Message = "Skill found"
        };

        _workerSkillServiceMock.Setup(s => s.GetSkillByIdAsync(skillId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetSkillById(skillId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _workerSkillServiceMock.Verify(s => s.GetSkillByIdAsync(skillId), Times.Once);
    }

    [Test]
    public async Task GetSkillById_CallsServiceWithCorrectId()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var serviceResult = new ServiceResult<WorkerSkillResponse>
        {
            Data = CreateValidWorkerSkillResponse(skillId, "Test")
        };

        _workerSkillServiceMock.Setup(s => s.GetSkillByIdAsync(It.IsAny<Guid>())).ReturnsAsync(serviceResult);

        // Act
        await _controller.GetSkillById(skillId);

        // Assert
        _workerSkillServiceMock.Verify(s => s.GetSkillByIdAsync(skillId), Times.Once);
    }

    #endregion
}
