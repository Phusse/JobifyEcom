using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Users;
using JobifyEcom.Extensions;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

/// <summary>
/// Manages search operations for users. This controller provides endpoints to list or search users based on various criteria. Only authenticated users can access this endpoints.
/// </summary>
/// <param name="userService">Service for user-related operations.</param>
[Authorize]
[ApiController]
public class SearchController(IUserService userService) : ControllerBase
{
	private readonly IUserService _userService = userService;

	/// <summary>
	/// Lists or searches users with optional filtering, sorting, and cursor-based pagination.
	/// </summary>
	/// <remarks>
	/// Use this endpoint to retrieve a paginated list of users. You can provide filter parameters
	/// such as name, email, role, or status to narrow the results. Pagination is cursor-based,
	/// meaning you can navigate through large datasets efficiently using the returned cursors.
	/// Only authenticated users can access this endpoint.
	/// </remarks>
	/// <param name="request">Pagination, filtering, and sorting parameters for querying users.</param>
	/// <returns>A paged list of user summaries matching the query.</returns>
	/// <response code="200">Users retrieved successfully.</response>
	[ProducesResponseType(typeof(ApiResponse<CursorPaginationResponse<UserProfileSummaryResponse>>), StatusCodes.Status200OK)]
	[HttpGet(ApiRoutes.Search.Get.Users)]
	public async Task<IActionResult> SearchUsers([FromQuery] CursorPaginationRequest<UserProfileFilterRequest> request)
	{
		ServiceResult<CursorPaginationResponse<UserProfileSummaryResponse>> result = await _userService.SearchUsersAsync(request);
		return Ok(result.MapToApiResponse());
	}
}
