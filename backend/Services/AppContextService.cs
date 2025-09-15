using System.Net.Mail;
using JobifyEcom.Contracts.Errors;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;

namespace JobifyEcom.Services;

/// <summary>
/// Provides access to information about the currently authenticated user.
/// This service is intended to be used in request-scoped services
/// to retrieve the current user's ID or other context information.
/// </summary>
/// <param name="httpContextAccessor">
/// The <see cref="IHttpContextAccessor"/> used to access the current HTTP context.
/// </param>
internal class AppContextService(IHttpContextAccessor httpContextAccessor)
{
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

	/// <summary>
	/// Gets the unique identifier of the currently authenticated user.
	/// </summary>
	/// <returns>The <see cref="Guid"/> representing the authenticated user's ID.</returns>
	/// <exception cref="AppException">
	/// Thrown if no user is authenticated or the user ID cannot be determined.
	/// The exception uses <see cref="ErrorCatalog.Unauthorized"/> for consistent error reporting.
	/// </exception>
	internal Guid GetCurrentUserId()
	{
		return _httpContextAccessor.HttpContext?.User.GetUserId()
			?? throw new AppException(ErrorCatalog.Unauthorized);
	}
}
