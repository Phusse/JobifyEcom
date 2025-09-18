using System.Security.Claims;
using JobifyEcom.Contracts.Errors;
using JobifyEcom.Contracts.Results;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Users;
using JobifyEcom.Enums;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;
using JobifyEcom.Helpers;
using JobifyEcom.Models;
using JobifyEcom.Security;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// Provides operations for managing users, including retrieval, updates, and
/// other user-related functionality within the application.
/// </summary>
/// <param name="db">The database context used for data access.</param>
/// <param name="appContextService">The application context service.</param>
internal class UserService(AppDbContext db, AppContextService appContextService) : IUserService
{
	private readonly AppDbContext _db = db;
	private readonly AppContextService _appContextService = appContextService;

	public async Task<ServiceResult<UserProfileResponse>> GetCurrentUserAsync()
	{
		User user = await _appContextService.GetCurrentUserAsync();

		UserProfileResponse response = new()
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email,
			Bio = user.Bio ?? string.Empty,
			Roles = user.GetUserRoles(),
			CreatedAt = user.CreatedAt
		};

		return ServiceResult<UserProfileResponse>.Create(ResultCatalog.CurrentUserRetrieved, response);
	}

	public async Task<ServiceResult<object>> GetUserByIdAsync(Guid userId)
	{
		User? user = await _db.Users.FindAsync(userId)
			?? throw new AppException(ErrorCatalog.AccountNotFound);

		ClaimsPrincipal currentUserClaims = _appContextService.GetCurrentUserClaims();

		IReadOnlyList<string> roles = currentUserClaims.GetRoles();
		bool isAdmin = roles.Contains(SystemRole.Admin.ToString()) || roles.Contains(SystemRole.SuperAdmin.ToString());

		UserProfileResponse response = isAdmin switch
		{
			true => new AdminUserProfileResponse
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
			_ => new UserProfileResponse
			{
				Id = user.Id,
				Name = user.Name,
				Email = user.Email,
				Bio = user.Bio ?? string.Empty,
				Roles = user.GetUserRoles(),
				CreatedAt = user.CreatedAt,
			}
		};

		return ServiceResult<object>.Create(ResultCatalog.UserRetrieved, response);
	}

	public async Task<ServiceResult<object>> ConfirmEmailAsync(EmailConfirmRequest request)
	{
		string normalizedEmail = request.Email.ToLowerInvariant().Trim();

		User user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail)
			?? throw new AppException(ErrorCatalog.AccountNotFoundByEmail);

		if (user.IsEmailConfirmed)
		{
			return ServiceResult<object>.Create(ResultCatalog.EmailAlreadyConfirmed);
		}

		if (user.EmailConfirmationToken != request.Token)
		{
			throw new AppException(ErrorCatalog.EmailConfirmationTokenInvalid);
		}

		user.IsEmailConfirmed = true;
		user.EmailConfirmationToken = null;
		user.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(ResultCatalog.EmailConfirmed);
	}

	public async Task<ServiceResult<object>> DeleteCurrentUserAsync()
	{
		User? user = await _appContextService.GetCurrentUserAsync();

		_db.Users.Remove(user);
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(ResultCatalog.CurrentUserDeleted);
	}

	public async Task<ServiceResult<object>> DeleteUserAsync(Guid userId)
	{
		ClaimsPrincipal currentUserClaims = _appContextService.GetCurrentUserClaims();
		User? userToRemove = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId)
			?? throw new AppException(ErrorCatalog.AccountNotFound);

		EnsureCanModifyUser(userToRemove, currentUserClaims);

		_db.Users.Remove(userToRemove);
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(ResultCatalog.UserDeleted);
	}

	public async Task<ServiceResult<object>> LockUserAsync(Guid id)
	{
		User? user = await _db.Users.FindAsync(id)
			?? throw new AppException(ErrorCatalog.AccountNotFound);

		if (user.IsLocked)
		{
			return ServiceResult<object>.Create(ResultCatalog.UserAlreadyLocked);
		}

		ClaimsPrincipal currentUserClaims = _appContextService.GetCurrentUserClaims();
		EnsureCanModifyUser(user, currentUserClaims);

		user.IsLocked = true;
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(ResultCatalog.UserLocked);
	}

	public async Task<ServiceResult<object>> RequestPasswordResetAsync(Guid id)
	{
		User? user = await _db.Users.FindAsync(id)
			?? throw new AppException(ErrorCatalog.AccountNotFound);

		if (user.IsLocked)
		{
			throw new AppException(ErrorCatalog.PasswordResetUnavailableForLockedAccount);
		}

		user.PasswordResetToken = Guid.NewGuid();
		user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
		user.UpdatedAt = DateTime.UtcNow;
		await _db.SaveChangesAsync();

		// TODO: Email service to send token to user.Email and return in response

		return ServiceResult<object>.Create(
			ResultCatalog.PasswordResetRequested.AppendDetails(
				"Email sending is not yet implemented. For now, the reset token and expirty are returned in the response."
			),
			new
			{
				resetToken = user.PasswordResetToken,
				resetTokenExpiry = user.PasswordResetTokenExpiry,
			}
		);
	}

	public async Task<ServiceResult<object>> ResetPasswordAsync(Guid id, PasswordResetRequest request)
	{
		User? user = await _db.Users.FindAsync(id)
			?? throw new AppException(ErrorCatalog.AccountNotFound);

		if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
		{
			throw new AppException(ErrorCatalog.PasswordResetLinkExpired);
		}

		if (string.IsNullOrWhiteSpace(request.NewPassword))
		{
			throw new AppException(ErrorCatalog.PasswordResetMissingNewPassword);
		}

		user.PasswordHash = PasswordSecurity.HashPassword(request.NewPassword);
		user.PasswordResetToken = null;
		user.PasswordResetTokenExpiry = null;
		user.SecurityStamp = Guid.Empty;
		user.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(ResultCatalog.PasswordResetSuccessful);
	}

	public async Task<ServiceResult<object>> UnlockUserAsync(Guid id)
	{
		User? user = await _db.Users.FindAsync(id)
			?? throw new AppException(ErrorCatalog.AccountNotFound);

		if (!user.IsLocked)
		{
			return ServiceResult<object>.Create(ResultCatalog.UserAlreadyUnlocked);
		}

		ClaimsPrincipal currentUserClaims = _appContextService.GetCurrentUserClaims();
		EnsureCanModifyUser(user, currentUserClaims);

		user.IsLocked = false;
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(ResultCatalog.UserUnlocked);
	}

	public async Task<ServiceResult<UserProfileResponse>> UpdateCurrentUserAsync(UserProfileUpdateRequest request)
	{
		User user = await _appContextService.GetCurrentUserAsync();

		bool isUpdated = false;

		isUpdated |= UpdateHelper.TryUpdate(request.Name, u => user.Name = u);
		isUpdated |= UpdateHelper.TryUpdate(request.Bio, u => user.Bio = u);

		if (isUpdated)
		{
			user.UpdatedAt = DateTime.UtcNow;
			await _db.SaveChangesAsync();
		}

		UserProfileResponse response = new()
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email,
			Bio = user.Bio ?? string.Empty,
			Roles = user.GetUserRoles(),
			CreatedAt = user.CreatedAt,
		};

		return ServiceResult<UserProfileResponse>.Create(ResultCatalog.UserProfileUpdated, response);
	}

	private static void EnsureCanModifyUser(User targetUser, ClaimsPrincipal currentUserPrincipal)
	{
		Guid currentUserId = currentUserPrincipal.GetUserId()
			?? throw new AppException(ErrorCatalog.Unauthorized);

		// Prevent user from modifying themselves
		if (targetUser.Id == currentUserId)
		{
			throw new AppException(ErrorCatalog.SelfModificationNotAllowed);
		}

		IReadOnlyList<string> currentUserRoles = currentUserPrincipal.GetRoles();

		// Only allow admins to modify workers and baseline users, but not SuperAdmins or Admins
		if (currentUserRoles.Contains(SystemRole.Admin.ToString()))
		{
			if (targetUser.StaffRole == SystemRole.SuperAdmin)
			{
				throw new AppException(ErrorCatalog.AdminCannotModifySuperAdmin);
			}
			if (targetUser.StaffRole == SystemRole.Admin)
			{
				throw new AppException(ErrorCatalog.AdminCannotModifyAdmin);
			}
		}

		// Ensure only SuperAdmins can modify Admins or SuperAdmins
		if (targetUser.StaffRole == SystemRole.SuperAdmin
			&& !currentUserRoles.Contains(SystemRole.SuperAdmin.ToString()))
		{
			throw new AppException(ErrorCatalog.SuperAdminRequiredForModification);
		}
	}
}
