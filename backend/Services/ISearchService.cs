using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Users;

namespace JobifyEcom.Services;

/// <summary>
/// Defines operations for searching entities, such as users, with support for pagination and filtering.
/// </summary>
public interface ISearchService
{
	/// <summary>
	/// Searches and lists users using pagination and optional filtering.
	/// </summary>
	/// <param name="request">The pagination and filter criteria.</param>
	/// <returns>A paginated list of user summaries.</returns>
	Task<ServiceResult<CursorPaginationResponse<UserProfileSummaryResponse>>> SearchUsersAsync(CursorPaginationRequest<UserProfileFilterRequest> request);
}
