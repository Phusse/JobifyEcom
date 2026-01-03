using Microsoft.AspNetCore.Mvc;
using Moq;
using JobifyEcom.Controllers;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Users;
using JobifyEcom.Services;

namespace JobifyEcom.Tests.Controllers;

[TestFixture]
public class SearchControllerTests
{
    private Mock<ISearchService> _searchServiceMock = null!;
    private SearchController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _searchServiceMock = new Mock<ISearchService>();
        _controller = new SearchController(_searchServiceMock.Object);
    }

    #region SearchUsers Tests

    [Test]
    public async Task SearchUsers_WithValidRequest_ReturnsOkWithPaginatedResults()
    {
        // Arrange
        var request = new CursorPaginationRequest<UserProfileFilterRequest>
        {
            PageSize = 10,
            Filter = new UserProfileFilterRequest { SearchTerm = "John" }
        };
        var users = new List<UserProfileSummaryResponse>
        {
            new() { Id = Guid.NewGuid(), Name = "John Doe" },
            new() { Id = Guid.NewGuid(), Name = "John Smith" }
        };
        var paginatedResponse = new CursorPaginationResponse<UserProfileSummaryResponse>
        {
            Items = users,
            NextCursor = null,
            HasMore = false
        };
        var serviceResult = new ServiceResult<CursorPaginationResponse<UserProfileSummaryResponse>>
        {
            Data = paginatedResponse,
            Message = "Users retrieved"
        };

        _searchServiceMock.Setup(s => s.SearchUsersAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.SearchUsers(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _searchServiceMock.Verify(s => s.SearchUsersAsync(request), Times.Once);
    }

    [Test]
    public async Task SearchUsers_WithEmptyResults_ReturnsOkWithEmptyList()
    {
        // Arrange
        var request = new CursorPaginationRequest<UserProfileFilterRequest>
        {
            PageSize = 10,
            Filter = new UserProfileFilterRequest { SearchTerm = "NonExistent" }
        };
        var paginatedResponse = new CursorPaginationResponse<UserProfileSummaryResponse>
        {
            Items = [],
            NextCursor = null,
            HasMore = false
        };
        var serviceResult = new ServiceResult<CursorPaginationResponse<UserProfileSummaryResponse>>
        {
            Data = paginatedResponse,
            Message = "No users found"
        };

        _searchServiceMock.Setup(s => s.SearchUsersAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.SearchUsers(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task SearchUsers_WithPagination_UsesCorrectCursor()
    {
        // Arrange
        var request = new CursorPaginationRequest<UserProfileFilterRequest>
        {
            PageSize = 5,
            Cursor = "encoded-cursor"
        };
        var serviceResult = new ServiceResult<CursorPaginationResponse<UserProfileSummaryResponse>>
        {
            Data = new CursorPaginationResponse<UserProfileSummaryResponse>
            {
                Items = [],
                NextCursor = "next-cursor",
                HasMore = true
            }
        };

        _searchServiceMock.Setup(s => s.SearchUsersAsync(It.IsAny<CursorPaginationRequest<UserProfileFilterRequest>>())).ReturnsAsync(serviceResult);

        // Act
        await _controller.SearchUsers(request);

        // Assert
        _searchServiceMock.Verify(s => s.SearchUsersAsync(It.Is<CursorPaginationRequest<UserProfileFilterRequest>>(r => 
            r.Cursor == "encoded-cursor" && r.PageSize == 5)), Times.Once);
    }

    #endregion
}
