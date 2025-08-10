using System.Security.Claims;
using System.Text.Json;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Helpers;

/// <summary>
/// Provides factory methods for configuring JWT authentication event handlers.
/// </summary>
public static class JwtEventHandlers
{
	/// <summary>
	/// Creates a <see cref="JwtBearerEvents"/> instance with custom responses for unauthorized and forbidden requests.
	/// </summary>
	/// <param name="jsonOptions">The JSON serialization options used to format the response.</param>
	/// <returns>
	/// A configured <see cref="JwtBearerEvents"/> instance that returns standardized JSON error responses
	/// for <c>401 Unauthorized</c> and <c>403 Forbidden</c> scenarios.
	/// </returns>
	/// <remarks>
	/// This is useful for returning consistent API responses when token validation fails or access is denied.
	/// </remarks>
	public static JwtBearerEvents Create(JsonSerializerOptions jsonOptions)
	{
		return new JwtBearerEvents
		{
			OnTokenValidated = async context =>
			{
				var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var securityStampClaim = context.Principal?.FindFirst("security_stamp")?.Value;

				if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(securityStampClaim))
				{
					context.Fail("Invalid token claims.");
					return;
				}

				// Resolve DbContext from DI
				var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

				if (!Guid.TryParse(userIdClaim, out var userId))
				{
					context.Fail("Invalid user ID claim.");
					return;
				}

				var user = await db.Users.AsNoTracking()
										.Where(u => u.Id == userId)
										.Select(u => new { u.SecurityStamp })
										.FirstOrDefaultAsync();

				if (user == null)
				{
					context.Fail("User not found.");
					return;
				}

				if (user.SecurityStamp == Guid.Empty)
				{
					context.Fail("User logged out - invalid security stamp.");
					return;
				}

				if (user.SecurityStamp.ToString() != securityStampClaim)
				{
					context.Fail("Security stamp mismatch. Token is invalid.");
					return;
				}

			},
			OnChallenge = context =>
			{
				context.HandleResponse();
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				context.Response.ContentType = "application/json";

				var response = ApiResponse<object>.Fail(null, "Unauthorized access.", ["Invalid or missing token."]);
				string json = JsonSerializer.Serialize(response, jsonOptions);

				return context.Response.WriteAsync(json);
			},
			OnForbidden = context =>
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				context.Response.ContentType = "application/json";

				var response = ApiResponse<object>.Fail(null, "Unauthorized access.", ["You do not have access to this resource."]);
				string json = JsonSerializer.Serialize(response, jsonOptions);

				return context.Response.WriteAsync(json);
			}
		};
	}
}
