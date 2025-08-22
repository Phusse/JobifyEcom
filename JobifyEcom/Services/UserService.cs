using System.Security.Claims;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.User;
using JobifyEcom.Enums;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;
using JobifyEcom.Helpers;
using JobifyEcom.Models;
using JobifyEcom.Security;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// User service for managing user-related operations.
/// </summary>
/// <param name="db">The database context.</param>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
/// <param name="cursorProtector">The cursor protector.</param>
internal class UserService(AppDbContext db, IHttpContextAccessor httpContextAccessor, CursorProtector cursorProtector) : IUserService
{
	private readonly AppDbContext _db = db;
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
	private readonly CursorProtector _cursorProtector = cursorProtector;
	private const int MaxCursorDepth = 20;

	public async Task<ServiceResult<ProfileResponse>> GetCurrentUserAsync()
	{
		ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
			?? throw new UnauthorizedException(
				"Sign in required.",
				["You need to be signed in to access your account."]
			);

		Guid currentUserId = currentUserPrincipal.GetUserId()
			?? throw new UnauthorizedException(
				"Sign in required.",
				["You need to be signed in to access your account."]
			);

		// Include the WorkerProfile data else roles wont get populated properly
		// This is necessary to ensure the user's roles are correctly retrieved
		User? user = await _db.Users.Include(w => w.WorkerProfile).FirstOrDefaultAsync(u => u.Id == currentUserId)
			?? throw new NotFoundException(
				"Account not found.",
				["We couldn't find your account. Please contact support if this issue continues."]
			);

		ProfileResponse response = new()
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email,
			Bio = user.Bio ?? string.Empty,
			Roles = user.GetUserRoles(),
			CreatedAt = user.CreatedAt
		};

		return ServiceResult<ProfileResponse>.Create(response, "User retrieved successfully.");
	}

	public async Task<ServiceResult<object>> GetUserByIdAsync(Guid userId)
	{
		User? user = await _db.Users.FindAsync(userId)
			?? throw new NotFoundException("User not found.", ["No user exists with the specified ID."]);

		ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext?.User
			?? throw new UnauthorizedException("Authentication required.", ["You must be signed in."]);

		IReadOnlyList<string> roles = currentUser.GetRoles();
		bool isAdmin = roles.Contains(SystemRole.Admin.ToString()) || roles.Contains(SystemRole.SuperAdmin.ToString());

		ProfileResponse response = isAdmin switch
		{
			true => new AdminProfileResponse
			{
				Id = user.Id,
				Name = user.Name,
				Email = user.Email,
				Bio = user.Bio ?? string.Empty,
				Roles = user.GetUserRoles(),
				CreatedAt = user.CreatedAt,
				IsEmailConfirmed = user.IsEmailConfirmed,
				IsLocked = user.IsLocked,
				LockedAt = user.LockedAt,
				UpdatedAt = user.UpdatedAt,
			},
			_ => new ProfileResponse
			{
				Id = user.Id,
				Name = user.Name,
				Email = user.Email,
				Bio = user.Bio ?? string.Empty,
				Roles = user.GetUserRoles(),
				CreatedAt = user.CreatedAt,
			}
		};

		return ServiceResult<object>.Create(response, "User retrieved successfully.");
	}

	public async Task<ServiceResult<object>> ConfirmEmailAsync(EmailConfirmRequest request)
	{
		if (string.IsNullOrWhiteSpace(request?.Email))
		{
			throw new ValidationException(
				"Unable to confirm email.",
				["Please provide a valid email address to continue."]
			);
		}

		if (request.Token is null)
		{
			throw new ValidationException(
				"Unable to confirm email.",
				["The confirmation link is missing or invalid. Please request a new link and try again."]
			);
		}

		string normalizedEmail = request.Email.ToLowerInvariant().Trim();

		User user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail)
			?? throw new NotFoundException(
				"Unable to confirm email.",
				["No account found with this email address. Please check and try again."]
			);

		if (user.IsEmailConfirmed)
		{
			return ServiceResult<object>.Create(null, "Your email is already confirmed. You can now sign in.");
		}

		if (user.EmailConfirmationToken is null || user.EmailConfirmationToken != request.Token)
		{
			throw new ValidationException(
				"Unable to confirm email.",
				["This confirmation link is no longer valid. Please request a new one."]
			);
		}

		user.IsEmailConfirmed = true;
		user.EmailConfirmationToken = null;
		user.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "Success! Your email address has been confirmed.");
	}

	public async Task<ServiceResult<object>> DeleteCurrentUserAsync()
	{
		ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
			?? throw new UnauthorizedException(
				"Sign in required.",
				["You need to be signed in to delete your account."]
			);

		Guid currentUserId = currentUserPrincipal.GetUserId()
			?? throw new UnauthorizedException(
				"Sign in required.",
				["You need to be signed in to delete your account."]
			);

		User? user = await _db.Users.FindAsync(currentUserId)
			?? throw new NotFoundException(
				"Account not found.",
				["We couldn't find your account. Please contact support if this issue continues."]
			);

		_db.Users.Remove(user);
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "Your account has been deleted. We're sorry to see you go.");
	}

	public async Task<ServiceResult<object>> DeleteUserAsync(Guid userId)
	{
		User? userToRemove = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId)
			?? throw new NotFoundException(
				"User not found.",
				["No user exists with the specified ID."]
			);

		ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
			?? throw new UnauthorizedException(
				"Authentication required.",
				["You must be signed in to perform this action."]
			);

		EnsureCanModifyUser(userToRemove, currentUserPrincipal);

		_db.Users.Remove(userToRemove);
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "The account was successfully deleted.");
	}

	public async Task<ServiceResult<object>> LockUserAsync(Guid id)
	{
		User? user = await _db.FindAsync<User>(id)
			?? throw new NotFoundException(
				"User not found.",
				["No user found with the specified ID."]
			);

		ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
			?? throw new UnauthorizedException(
				"Authentication required.",
				["You must be signed in to perform this action."]
			);

		EnsureCanModifyUser(user, currentUserPrincipal);

		if (user.IsLocked)
		{
			return ServiceResult<object>.Create(null, "This user account is already locked.");
		}

		user.IsLocked = true;
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "User account has been locked.");
	}

	public async Task<ServiceResult<object>> RequestPasswordResetAsync(Guid id)
	{
		User user = await _db.Users.FindAsync(id)
			?? throw new NotFoundException("User not found.", ["No account found with this ID."]);

		if (user.IsLocked)
		{
			throw new ValidationException("Password reset unavailable.", ["This account is locked and cannot reset the password."]);
		}

		user.PasswordResetToken = Guid.NewGuid();
		user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
		user.UpdatedAt = DateTime.UtcNow;
		await _db.SaveChangesAsync();

		// TODO: Email service to send token to user.Email and return in response
		List<string> warnings =
		[
			"Email sending is not yet implemented. For now, the reset token and expiry are returned in the response."
		];

		return ServiceResult<object>.Create(new
		{
			resetToken = user.PasswordResetToken,
			resetTokenExpiry = user.PasswordResetTokenExpiry
		}, "A password reset link has been sent to your email address.", warnings);
	}

	public async Task<ServiceResult<object>> ResetPasswordAsync(Guid id, PasswordResetRequest request)
	{
		User user = await _db.Users.FindAsync(id)
			?? throw new NotFoundException("User not found.", ["No account found with this ID."]);

		if (user.PasswordResetToken is null || user.PasswordResetToken != request.Token)
		{
			throw new ValidationException("Password reset failed.", ["The reset link is invalid or has already been used."]);
		}

		if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
		{
			throw new ValidationException("Password reset failed.", ["The reset link has expired. Please request a new one."]);
		}

		if (string.IsNullOrWhiteSpace(request.NewPassword))
		{
			throw new ValidationException("Password reset failed.", ["Please enter a new password."]);
		}

		user.PasswordHash = PasswordSecurity.HashPassword(request.NewPassword);
		user.PasswordResetToken = null;
		user.PasswordResetTokenExpiry = null;
		user.SecurityStamp = Guid.Empty;
		user.UpdatedAt = DateTime.UtcNow;
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "Your password has been updated. You can now sign in with your new password.");
	}

	public async Task<ServiceResult<CursorPaginationResponse<ProfileSummaryResponse>>> SearchUsersAsync(CursorPaginationRequest<ProfileFilterRequest> request)
	{
		CursorState? cursorState = null;
		CursorPaginationResponse<ProfileSummaryResponse> response;

		// If cursor provided, decode it and use its state
		if (!string.IsNullOrEmpty(request.Cursor))
		{
			cursorState = _cursorProtector.Decode(request.Cursor);

			// Stop if max depth reached
			if (cursorState.Depth >= MaxCursorDepth)
			{
				response = new CursorPaginationResponse<ProfileSummaryResponse>()
				{
					NextCursor = null,
					HasMore = false,
					Items = [],
				};

				return ServiceResult<CursorPaginationResponse<ProfileSummaryResponse>>.Create(response, "No more results.");
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
				else if (IsValidEmail(search))
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

		List<ProfileSummaryResponse> items = [.. users.Select(u => new ProfileSummaryResponse
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
				Filter = request.Filter ?? new ProfileFilterRequest(),
				Depth = (cursorState?.Depth ?? 0) + 1
			};

			if (nextState.Depth < MaxCursorDepth)
			{
				nextCursor = _cursorProtector.Encode(nextState);
				hasMore = true;
			}
		}

		response = new CursorPaginationResponse<ProfileSummaryResponse>()
		{
			NextCursor = nextCursor,
			HasMore = hasMore,
			Items = items,
		};

		return ServiceResult<CursorPaginationResponse<ProfileSummaryResponse>>.Create(response, "Users found.");
	}

	public async Task<ServiceResult<object>> UnlockUserAsync(Guid id)
	{
		User? user = await _db.FindAsync<User>(id)
			?? throw new NotFoundException(
				"User not found.",
				["No user found with the specified ID."]
			);

		ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
			?? throw new UnauthorizedException(
				"Authentication required.",
				["You must be signed in to perform this action."]
			);

		EnsureCanModifyUser(user, currentUserPrincipal);

		if (!user.IsLocked)
		{
			return ServiceResult<object>.Create(null, "This user account is not locked.");
		}

		user.IsLocked = false;
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "User account has been unlocked.");
	}

	public async Task<ServiceResult<ProfileResponse>> UpdateCurrentUserAsync(ProfileUpdateRequest request)
	{
		if (request is null)
		{
			throw new ValidationException(
				"Update failed.",
				["Update request cannot be empty."]
			);
		}

		ClaimsPrincipal claimsPrincipal = _httpContextAccessor.HttpContext?.User
			?? throw new UnauthorizedException(
				"Authentication required.",
				["You must be signed in to perform this action."]
			);

		Guid? currentUserId = claimsPrincipal.GetUserId();
		User user = await _db.Users.FindAsync(currentUserId)
			?? throw new NotFoundException(
				"User not found.",
				["No user found with the specified ID."]
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
			Bio = user.Bio ?? string.Empty,
			Roles = user.GetUserRoles(),
			CreatedAt = user.CreatedAt,
		}, "User profile updated successfully.");
	}

	private static void EnsureCanModifyUser(User targetUser, ClaimsPrincipal currentUserPrincipal)
	{
		Guid currentUserId = currentUserPrincipal.GetUserId()
			?? throw new UnauthorizedException(
				"Authentication required.",
				["You must be signed in to perform this action."]
			);

		// Prevent user from modifying themselves
		if (targetUser.Id == currentUserId)
		{
			throw new ForbiddenException(
				"Operation not permitted.",
				["You cannot perform this action on your own account using this endpoint."]
			);
		}

		IReadOnlyList<string> currentUserRoles = currentUserPrincipal.GetRoles();

		// Only allow admins to modify workers and baseline users, but not SuperAdmins or Admins
		if (currentUserRoles.Contains(SystemRole.Admin.ToString()))
		{
			if (targetUser.StaffRole == SystemRole.SuperAdmin)
			{
				throw new ForbiddenException(
					"Operation not permitted.",
					["Admins cannot modify SuperAdmin accounts."]
				);
			}
			if (targetUser.StaffRole == SystemRole.Admin)
			{
				throw new ForbiddenException(
					"Operation not permitted.",
					["Admins cannot modify other Admin accounts."]
				);
			}
		}

		// Ensure only SuperAdmins can modify Admins or SuperAdmins
		if (targetUser.StaffRole == SystemRole.SuperAdmin
			&& !currentUserRoles.Contains(SystemRole.SuperAdmin.ToString()))
		{
			throw new ForbiddenException(
				"Operation not permitted.",
				["Only SuperAdmins can modify SuperAdmin accounts."]
			);
		}
	}

	private static bool IsValidEmail(string email)
	{
		try
		{
			var addr = new System.Net.Mail.MailAddress(email);
			return addr.Address == email;
		}
		catch { return false; }
	}
}
