using JobifyEcom.Contracts.Errors;
using JobifyEcom.Data;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;
using JobifyEcom.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Security;

/// <summary>
/// Performs validation of JWT access tokens after they have been successfully decoded.
/// Ensures that the token contains the correct claims, that the associated user exists,
/// and that their security stamp and roles are still valid.
/// </summary>
public static class TokenValidator
{
	/// <summary>
	/// Validates the claims contained in a JWT access token against the current database state.
	/// </summary>
	/// <param name="context">
	/// The <see cref="TokenValidatedContext"/> provided by the JWT bearer authentication middleware.
	/// Contains the <see cref="System.Security.Claims.ClaimsPrincipal"/> and request services.
	/// </param>
	/// <remarks>
	/// This method performs several checks:
	/// <list type="bullet">
	/// <item>Ensures the token contains a valid <c>userId</c> and <c>securityStamp</c> claim.</item>
	/// <item>Verifies that the token type is <c>Access</c>.</item>
	/// <item>Ensures the user still exists in the database and is not locked or removed.</item>
	/// <item>Checks that the roles in the token exactly match the roles from the database.</item>
	/// <item>Confirms that the security stamp has not changed (which would invalidate the session).</item>
	/// </list>
	/// </remarks>
	/// <exception cref="AppException">
	/// Thrown when any of the above checks fail. The exception contains a specific
	/// <see cref="ErrorCatalog"/> entry describing the failure, which is handled by
	/// the global exception middleware to produce a structured JSON error response.
	/// </exception>
	public static async Task ValidateAsync(TokenValidatedContext context)
	{
		if (context.Principal is null)
		{
			throw new AppException(ErrorCatalog.SessionNotVerified);
		}

		string? tokenType = context.Principal.GetTokenType();

		if (!string.Equals(tokenType, TokenType.Access.ToString(), StringComparison.OrdinalIgnoreCase))
		{
			throw new AppException(ErrorCatalog.InvalidSession);
		}

		Guid? userId = context.Principal.GetUserId();
		Guid? securityStamp = context.Principal.GetSecurityStamp();

		if (userId is null || securityStamp is null)
		{
			throw new AppException(ErrorCatalog.SessionNotVerified);
		}

		var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

		User user = await db.Users.AsNoTracking()
			.Include(u => u.WorkerProfile)
			.FirstOrDefaultAsync(u => u.Id == userId)
			?? throw new AppException(ErrorCatalog.AccountNotFound);

		IEnumerable<string> userRoles = user.GetUserRoles().Select(r => r.ToString());

		if (!RoleValidator.Validate(userRoles, context.Principal.GetRoles()))
		{
			throw new AppException(ErrorCatalog.RoleMismatch);
		}

		if (user.IsLocked)
			throw new AppException(ErrorCatalog.AccountLocked);

		if (user.SecurityStamp == Guid.Empty)
			throw new AppException(ErrorCatalog.SignedOut);

		if (user.SecurityStamp == securityStamp)
			throw new AppException(ErrorCatalog.SessionExpired);
	}
}
