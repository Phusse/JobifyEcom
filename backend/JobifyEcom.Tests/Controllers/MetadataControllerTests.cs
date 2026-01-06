using Microsoft.AspNetCore.Mvc;
using Moq;
using JobifyEcom.Controllers;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Metadata;
using JobifyEcom.Services;

namespace JobifyEcom.Tests.Controllers;

[TestFixture]
public class MetadataControllerTests
{
    private Mock<IMetadataService> _metadataServiceMock = null!;
    private MetadataController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _metadataServiceMock = new Mock<IMetadataService>();
        _controller = new MetadataController(_metadataServiceMock.Object);
    }

    #region GetAllEnums Tests

    [Test]
    public void GetAllEnums_ReturnsOkWithEnumList()
    {
        // Arrange
        var enumSets = new List<EnumSetResponse>
        {
            new() { Name = "SystemRole", Values = [new() { Key = "User", DisplayName = "User" }] },
            new() { Name = "JobStatus", Values = [new() { Key = "Open", DisplayName = "Open" }] }
        };
        var serviceResult = new ServiceResult<List<EnumSetResponse>>
        {
            Data = enumSets,
            Message = "Enums retrieved"
        };

        _metadataServiceMock.Setup(s => s.GetAllEnums()).Returns(serviceResult);

        // Act
        var result = _controller.GetAllEnums();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _metadataServiceMock.Verify(s => s.GetAllEnums(), Times.Once);
    }

    [Test]
    public void GetAllEnums_ReturnsEmptyList_WhenNoEnumsRegistered()
    {
        // Arrange
        var serviceResult = new ServiceResult<List<EnumSetResponse>>
        {
            Data = [],
            Message = "No enums found"
        };

        _metadataServiceMock.Setup(s => s.GetAllEnums()).Returns(serviceResult);

        // Act
        var result = _controller.GetAllEnums();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    #endregion

    #region GetEnumByType Tests

    [Test]
    public void GetEnumByType_WithValidType_ReturnsOkWithEnum()
    {
        // Arrange
        var enumTypeName = "SystemRole";
        var enumSet = new EnumSetResponse 
        { 
            Name = "SystemRole", 
            Values = [new() { Key = "User", DisplayName = "User" }, new() { Key = "Admin", DisplayName = "Admin" }] 
        };
        var serviceResult = new ServiceResult<EnumSetResponse?>
        {
            Data = enumSet,
            Message = "Enum found"
        };

        _metadataServiceMock.Setup(s => s.GetEnumByType(enumTypeName)).Returns(serviceResult);

        // Act
        var result = _controller.GetEnumByType(enumTypeName);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _metadataServiceMock.Verify(s => s.GetEnumByType(enumTypeName), Times.Once);
    }

    [Test]
    public void GetEnumByType_WithInvalidType_ReturnsOkWithNullData()
    {
        // Arrange
        var enumTypeName = "InvalidEnum";
        var serviceResult = new ServiceResult<EnumSetResponse?>
        {
            Data = null,
            Message = "Enum not found"
        };

        _metadataServiceMock.Setup(s => s.GetEnumByType(enumTypeName)).Returns(serviceResult);

        // Act
        var result = _controller.GetEnumByType(enumTypeName);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public void GetEnumByType_IsCaseInsensitive_CallsServiceWithOriginalInput()
    {
        // Arrange
        var enumTypeName = "SYSTEMROLE";
        var serviceResult = new ServiceResult<EnumSetResponse?> { Data = null };

        _metadataServiceMock.Setup(s => s.GetEnumByType(It.IsAny<string>())).Returns(serviceResult);

        // Act
        _controller.GetEnumByType(enumTypeName);

        // Assert
        _metadataServiceMock.Verify(s => s.GetEnumByType("SYSTEMROLE"), Times.Once);
    }

    #endregion
}
