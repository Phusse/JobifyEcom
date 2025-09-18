using JobifyEcom.Contracts.Results;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Users;
using JobifyEcom.Enums;
using JobifyEcom.Helpers;
using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// Provides search functionality for users with support for filtering, sorting, and cursor-based pagination.
/// This service allows querying entities dynamically and returning paginated results in a safe, stateless manner.
/// </summary>
/// <param name="db">The application database context used to query users.</param>
/// <param name="appContextService">The service providing information about the current authenticated user.</param>
/// <param name="cursorProtector">A helper used to encode and decode cursor state for pagination.</param>
internal class SearchService(AppDbContext db, AppContextService appContextService, CursorProtector cursorProtector) : ISearchService
{
	private readonly AppDbContext _db = db;
	private readonly AppContextService _appContextService = appContextService;
	private readonly CursorProtector _cursorProtector = cursorProtector;

	private const int MaxCursorDepth = 20;

	public async Task<ServiceResult<CursorPaginationResponse<UserProfileSummaryResponse>>> SearchUsersAsync(CursorPaginationRequest<UserProfileFilterRequest> request)
	{
		CursorState? cursorState = null;
		CursorPaginationResponse<UserProfileSummaryResponse> response;

		// If cursor provided, decode it and use its state
		if (!string.IsNullOrWhiteSpace(request.Cursor))
		{
			cursorState = _cursorProtector.Decode(request.Cursor);

			// Stop if max depth reached
			if (cursorState.Depth >= MaxCursorDepth)
			{
				response = new CursorPaginationResponse<UserProfileSummaryResponse>()
				{
					NextCursor = null,
					HasMore = false,
					Items = [],
				};

				return ServiceResult<CursorPaginationResponse<UserProfileSummaryResponse>>.Create(
					ResultCatalog.MaxCursorDepthReached, response
				);
			}

			request.Filter = cursorState.Filter;
		}

		IQueryable<User> query = _db.Users.AsNoTracking();

		// Apply Search
		if (!string.IsNullOrWhiteSpace(request.Filter?.SearchTerm))
		{
			string search = request.Filter.SearchTerm.Trim();
			UserSearchField searchBy = request.Filter.SearchBy;

			// Determine search field
			if (searchBy == UserSearchField.Auto)
			{
				if (Guid.TryParse(search, out _))
				{
					searchBy = UserSearchField.Id;
				}
				else if (ValidationHelper.IsValidEmail(search))
				{
					searchBy = UserSearchField.Email;
				}
				else
				{
					searchBy = UserSearchField.Name;
				}
			}

			query = searchBy switch
			{
				UserSearchField.Id => Guid.TryParse(search, out var guid)
					? query.Where(u => u.Id == guid)
					: query.Where(u => false),

				UserSearchField.Email => query
					.Where(u => u.Email.ToLower() == search.ToLower()),

				UserSearchField.Name => query
					.Where(u => u.Name.ToLower().Contains(search.ToLower())),

				_ => query,
			};
		}

		// Apply Sorting
		bool descending = request.Filter?.SortDescending ?? false;
		query = request.Filter?.SortBy switch
		{
			UserSortField.Name => descending
				? query.OrderByDescending(u => u.Name).ThenByDescending(u => u.Id)
				: query.OrderBy(u => u.Name).ThenBy(u => u.Id),

			UserSortField.Id => descending
				? query.OrderByDescending(u => u.Id)
				: query.OrderBy(u => u.Id),

			_ => descending
				? query.OrderByDescending(u => u.CreatedAt).ThenByDescending(u => u.Id)
				: query.OrderBy(u => u.CreatedAt).ThenBy(u => u.Id),
		};

		// Apply cursor position (if any)
		if (cursorState is not null)
		{
			CursorState last = cursorState;

			if (descending)
			{
				query = query.Where(u =>
					(u.CreatedAt < last.LastCreatedAt) ||
					(u.CreatedAt == last.LastCreatedAt && u.Id.CompareTo(last.LastId) < 0));
			}
			else
			{
				query = query.Where(u =>
					(u.CreatedAt > last.LastCreatedAt) ||
					(u.CreatedAt == last.LastCreatedAt && u.Id.CompareTo(last.LastId) > 0));
			}
		}

		// Fetch Page
		List<User> users = await query.Take(request.PageSize).ToListAsync();

		List<UserProfileSummaryResponse> items = [.. users.Select(u => new UserProfileSummaryResponse
		{
			Id = u.Id,
			Name = u.Name,
		})];

		// Build Next Cursor
		string? nextCursor = null;
		bool hasMore = false;

		if (users.Count == request.PageSize)
		{
			User lastUser = users.Last();

			CursorState nextState = new()
			{
				LastCreatedAt = lastUser.CreatedAt,
				LastId = lastUser.Id,
				SortBy = request.Filter?.SortBy ?? UserSortField.CreatedAt,
				SortDescending = request.Filter?.SortDescending ?? false,
				Filter = request.Filter ?? new UserProfileFilterRequest(),
				Depth = (cursorState?.Depth ?? 0) + 1
			};

			if (nextState.Depth < MaxCursorDepth)
			{
				nextCursor = _cursorProtector.Encode(nextState);
				hasMore = true;
			}
		}

		response = new CursorPaginationResponse<UserProfileSummaryResponse>()
		{
			NextCursor = nextCursor,
			HasMore = hasMore,
			Items = items,
		};

		return ServiceResult<CursorPaginationResponse<UserProfileSummaryResponse>>.Create(
			response.Items.Count > 0 ? ResultCatalog.UsersFound : ResultCatalog.UsersNotFound,
			response
		);
	}
}
