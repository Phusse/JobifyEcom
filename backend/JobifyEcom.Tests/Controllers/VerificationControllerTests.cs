using Microsoft.AspNetCore.Mvc;
using Moq;
using JobifyEcom.Controllers;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;
using JobifyEcom.Services;
using JobifyEcom.Enums;

namespace JobifyEcom.Tests.Controllers;

[TestFixture]
public class VerificationControllerTests
{
    private Mock<IVerificationService> _verificationServiceMock = null!;
    private VerificationController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _verificationServiceMock = new Mock<IVerificationService>();
        _controller = new VerificationController(_verificationServiceMock.Object);
    }

    private static WorkerSkillResponse CreateValidWorkerSkillResponse(
        Guid? id = null, 
        string name = "C# Programming",
        VerificationStatus status = VerificationStatus.Pending) => new()
    {
        Id = id ?? Guid.NewGuid(),
        WorkerId = Guid.NewGuid(),
        Name = name,
        Description = "A skill description",
        Level = SkillLevel.Expert,
        YearsOfExperience = 5,
        CertificationLink = null,
        Tags = ["Backend", "C#"],
        VerificationStatus = status
    };

    #region VerifySkill Tests

    [Test]
    public async Task VerifySkill_WithApprovalRequest_ReturnsOkResult()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var request = new VerifySkillRequest 
        { 
            Status = VerificationStatus.Approved, 
            ReviewerComment = "Skill verified after review" 
        };
        var skillResponse = CreateValidWorkerSkillResponse(skillId, "C# Programming", VerificationStatus.Approved);
        var serviceResult = new ServiceResult<WorkerSkillResponse>
        {
            Data = skillResponse,
            Message = "Skill verified"
        };

        _verificationServiceMock.Setup(s => s.VerifySkillAsync(skillId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.VerifySkill(skillId, request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _verificationServiceMock.Verify(s => s.VerifySkillAsync(skillId, request), Times.Once);
    }

    [Test]
    public async Task VerifySkill_WithRejectionRequest_ReturnsOkResult()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var request = new VerifySkillRequest 
        { 
            Status = VerificationStatus.Rejected, 
            ReviewerComment = "Insufficient evidence provided" 
        };
        var skillResponse = CreateValidWorkerSkillResponse(skillId, "Test Skill", VerificationStatus.Rejected);
        var serviceResult = new ServiceResult<WorkerSkillResponse>
        {
            Data = skillResponse,
            Message = "Skill rejected"
        };

        _verificationServiceMock.Setup(s => s.VerifySkillAsync(skillId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.VerifySkill(skillId, request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task VerifySkill_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var request = new VerifySkillRequest 
        { 
            Status = VerificationStatus.Approved, 
            ReviewerComment = "Approved" 
        };
        var serviceResult = new ServiceResult<WorkerSkillResponse>
        {
            Data = CreateValidWorkerSkillResponse(skillId)
        };

        _verificationServiceMock.Setup(s => s.VerifySkillAsync(It.IsAny<Guid>(), It.IsAny<VerifySkillRequest>()))
            .ReturnsAsync(serviceResult);

        // Act
        await _controller.VerifySkill(skillId, request);

        // Assert
        _verificationServiceMock.Verify(s => s.VerifySkillAsync(
            skillId, 
            It.Is<VerifySkillRequest>(r => r.Status == VerificationStatus.Approved && r.ReviewerComment == "Approved")), 
            Times.Once);
    }

    #endregion
}
