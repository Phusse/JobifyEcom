using System.Security.Claims;
using JobifyEcom.Contracts.Errors;
using JobifyEcom.Data;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;
using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// Provides access to information about the currently authenticated user.
/// This service is intended to be used in request-scoped services
/// to retrieve the current user's ID or full profile information.
/// </summary>
/// <param name="httpContextAccessor">
/// The <see cref="IHttpContextAccessor"/> used to access the current HTTP context.
/// </param>
/// <param name="dbContext">
/// The application's database context used to load user information.
/// </param>
internal class AppContextService(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
{
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
	private readonly AppDbContext _db = dbContext;

	/// <summary>
	/// Gets the unique identifier of the currently authenticated user.
	/// </summary>
	/// <returns>The <see cref="Guid"/> representing the authenticated user's ID.</returns>
	/// <exception cref="AppException">
	/// Thrown if no user is authenticated or the user ID cannot be determined.
	/// </exception>
	internal Guid GetCurrentUserId()
	{
		return _httpContextAccessor.HttpContext?.User.GetUserId()
			?? throw new AppException(ErrorCatalog.Unauthorized);
	}

	/// <summary>
	/// Gets the claims of the currently authenticated user.
	/// </summary>
	/// <returns>The <see cref="ClaimsPrincipal"/> representing the authenticated user's claims.</returns>
	/// <exception cref="AppException">
	/// Thrown if no user is authenticated or the user claims cannot be determined.
	/// </exception>
	internal ClaimsPrincipal GetCurrentUserClaims()
	{
		return _httpContextAccessor.HttpContext?.User
			?? throw new AppException(ErrorCatalog.Unauthorized);
	}

	/// <summary>
	/// Loads the currently authenticated user's full data from the database,
	/// including their <see cref="Worker"/> (if it exists).
	/// </summary>
	/// <returns>The <see cref="User"/> entity of the currently logged-in user.</returns>
	/// <exception cref="AppException">
	/// Thrown if the user cannot be found in the database.
	/// </exception>
	internal async Task<User> GetCurrentUserAsync()
	{
		Guid currentUserId = GetCurrentUserId();
		User user = await _db.Users
			.Include(u => u.WorkerProfile)
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == currentUserId)
			?? throw new AppException(ErrorCatalog.AccountNotFound);

		return user;
	}
}
