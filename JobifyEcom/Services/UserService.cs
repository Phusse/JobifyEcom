using System.Security.Claims;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.User;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;
using JobifyEcom.Models;
using JobifyEcom.Security;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

internal class UserService(AppDbContext db, IHttpContextAccessor httpContextAccessor) : IUserService
{
	private readonly AppDbContext _db = db;
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

	public async Task<ServiceResult<object>> ConfirmEmailAsync(EmailConfirmRequest request)
	{
		if (string.IsNullOrWhiteSpace(request?.Email))
		{
			throw new ValidationException(
				"Email confirmation failed.",
				["We couldn't process your request because the email address is missing."]
			);
		}

		if (request.Token is null)
		{
			throw new ValidationException(
				"Email confirmation failed.",
				["Your confirmation link is missing or invalid. Please try again."]
			);
		}

		string normalizedEmail = request.Email.ToLowerInvariant().Trim();

		User user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail)
			?? throw new NotFoundException(
				"Email confirmation failed.",
				["We couldn't find an account with this email address."]
			);

		if (user.IsEmailConfirmed)
		{
			return ServiceResult<object>.Create(null, "Your email has already been confirmed.");
		}

		if (user.EmailConfirmationToken is null || user.EmailConfirmationToken != request.Token)
		{
			throw new ValidationException(
				"Email confirmation failed.",
				["This confirmation link is no longer valid. Please request a new one."]
			);
		}

		user.IsEmailConfirmed = true;
		user.EmailConfirmationToken = null;
		user.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "Thank you! Your email has been successfully confirmed.");
	}

	public async Task<ServiceResult<object>> DeleteUserAsync(Guid id)
	{
		User? user = await _db.Users.FindAsync(id)
			?? throw new NotFoundException(
				"User not found.",
				["We couldn't find a user with this ID."]
			);

		_db.Users.Remove(user);
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "User deleted successfully.");
	}

	public async Task<ServiceResult<ProfileResponse>> GetCurrentUserAsync()
	{
		ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
			?? throw new UnauthorizedException(
				"Authentication required.",
				["You must be logged in to access your account information."]
			);

		Guid currentUserId = currentUserPrincipal.GetUserId()
			?? throw new UnauthorizedException(
				"Authentication required.",
				["You must be logged in to perform this action."]
			);

		// Eagerly include the WorkerProfile to get the full user roles.
		User user = await _db.Users
			.AsNoTracking()
			.Include(u => u.WorkerProfile)
			.FirstOrDefaultAsync(u => u.Id == currentUserId)
			?? throw new NotFoundException(
				"User not found.",
				["We couldn't find your account. If this keeps happening, please contact support."]
			);

		ProfileResponse response = new()
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email,
			CreatedAt = user.CreatedAt,
			Roles = user.GetUserRoles(),
		};

		return ServiceResult<ProfileResponse>.Create(response, "User retrieved successfully.");
	}

	public async Task<ServiceResult<ProfileResponse>> GetUserByIdAsync(Guid id)
	{
		User? user = await _db.Users.FindAsync(id)
			?? throw new NotFoundException(
				"User not found.",
				["We couldn't find a user with this ID."]
			);

		ProfileResponse response = new()
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email,
			CreatedAt = user.CreatedAt,
			Roles = user.GetUserRoles(),
		};

		return ServiceResult<ProfileResponse>.Create(response, "User retrieved successfully.");
	}

	public async Task<ServiceResult<object>> LockUserAsync(Guid id)
	{
		User? user = await _db.FindAsync<User>(id)
			?? throw new NotFoundException(
				"User not found.",
				["We couldn't find a user with this ID."]
			);

		if (user.IsLocked)
		{
			return ServiceResult<object>.Create(null, "User is already locked.");
		}

		user.IsLocked = true;
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "User locked successfully.");
	}

	public async Task<ServiceResult<object>> RequestPasswordResetAsync(Guid id)
	{
		User user = await _db.Users.FindAsync(id)
			?? throw new NotFoundException("User not found.", ["No account found with this ID."]);

		if (user.IsLocked)
		{
			throw new ValidationException("Password reset failed.", ["This account is locked."]);
		}

		// Generate a reset token
		user.PasswordResetToken = Guid.NewGuid();
		user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
		user.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();

		// TODO: Email service to send token to user.Email
		// e.g. _emailService.SendPasswordResetLink(user.Email, user.PasswordResetToken)

		return ServiceResult<object>.Create(null, "A password reset link has been sent to your email.");
	}

	public async Task<ServiceResult<object>> ResetPasswordAsync(Guid id, PasswordResetRequest request)
	{
		User user = await _db.Users.FindAsync(id)
			?? throw new NotFoundException("User not found.", ["No account found with this ID."]);

		if (user.PasswordResetToken is null || user.PasswordResetToken != request.Token)
		{
			throw new ValidationException("Password reset failed.", ["This reset link is invalid or has expired."]);
		}

		if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
		{
			throw new ValidationException("Password reset failed.", ["This reset link has expired."]);
		}

		if (string.IsNullOrWhiteSpace(request.NewPassword))
		{
			throw new ValidationException("Password reset failed.", ["New password is required."]);
		}

		// Hash the new password
		user.PasswordHash = PasswordSecurity.HashPassword(request.NewPassword);
		user.PasswordResetToken = null;
		user.PasswordResetTokenExpiry = null;
		user.SecurityStamp = Guid.Empty; // force logout of active sessions
		user.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "Your password has been reset successfully.");
	}

	public async Task<ServiceResult<CursorPaginationResponse<ProfileSummaryResponse>>> SearchUsersAsync(
		CursorPaginationRequest<ProfileFilterRequest> request)
	{
		IQueryable<User> query = _db.Users.AsNoTracking();

		// Apply search filter
		if (!string.IsNullOrWhiteSpace(request.Filter?.SearchTerm))
		{
			string search = request.Filter.SearchTerm.Trim().ToLower();
			query = query.Where(u =>
				u.Name.ToLower().Contains(search) ||
				u.Email.ToLower().Contains(search)
			);
		}

		// Sorting
		string sortBy = request.Filter?.SortBy?.ToLower() ?? "createdat";
		bool descending = request.Filter?.SortDescending ?? false;

		query = sortBy switch
		{
			"name" => descending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
			"id" => descending ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id),
			_ => descending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
		};

		// Cursor
		if (!string.IsNullOrEmpty(request.Cursor))
		{
			if (DateTime.TryParse(request.Cursor, out DateTime cursorValue))
			{
				if (descending)
				{
					query = query.Where(u => u.CreatedAt < cursorValue);
				}
				else
				{
					query = query.Where(u => u.CreatedAt > cursorValue);
				}
			}
		}

		// Fetch page
		List<User> users = await query.Take(request.PageSize).ToListAsync();

		// Map to DTO
		List<ProfileSummaryResponse> items = users.Select(u => new ProfileSummaryResponse
		{
			Id = u.Id,
			Name = u.Name,
		}).ToList();

		// Compute next cursor
		string? nextCursor = users.Count == request.PageSize
			? users.Last().CreatedAt.ToString("O")
			: null;

		CursorPaginationResponse<ProfileSummaryResponse> response = new()
		{
			Items = items,
			NextCursor = nextCursor,
			HasMore = nextCursor is not null
		};

		return ServiceResult<CursorPaginationResponse<ProfileSummaryResponse>>.Create(response, "Users retrieved successfully.");
	}

	public async Task<ServiceResult<object>> UnlockUserAsync(Guid id)
	{
		User? user = await _db.FindAsync<User>(id)
			?? throw new NotFoundException(
				"User not found.",
				["We couldn't find a user with this ID."]
			);

		if (!user.IsLocked)
		{
			return ServiceResult<object>.Create(null, "User is not locked.");
		}

		user.IsLocked = false;
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "User unlocked successfully.");
	}

	public async Task<ServiceResult<ProfileResponse>> UpdateUserAsync(Guid id, ProfileUpdateRequest request)
	{
		if (request is null)
		{
			throw new ValidationException(
				"Update failed.",
				["The update request cannot be null."]
			);
		}

		User? user = await _db.Users.FindAsync(id)
			?? throw new NotFoundException(
				"User not found.",
				["We couldn't find a user with this ID."]
			);

		if (!string.IsNullOrWhiteSpace(request.Name))
		{
			user.Name = request.Name.Trim();
		}

		if (!string.IsNullOrWhiteSpace(request.Bio))
		{
			user.Bio = request.Bio.Trim();
		}

		user.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();

		return ServiceResult<ProfileResponse>.Create(new ProfileResponse
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email,
			Roles = user.GetUserRoles(),
			CreatedAt = user.CreatedAt,
		}, "User updated successfully.");
	}
}
