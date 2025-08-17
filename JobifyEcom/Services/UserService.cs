using System.Security.Claims;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.User;
using JobifyEcom.Enums;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;
using JobifyEcom.Models;
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

	public Task<ServiceResult<object>> DeleteUserAsync(Guid id)
	{
		throw new NotImplementedException();
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

		User user = await _db.Users
			.AsNoTracking()
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
			Role = user.Role,
		};

		return ServiceResult<ProfileResponse>.Create(response, "User retrieved successfully.");
	}

	public Task<ServiceResult<ProfileResponse>> GetUserByIdAsync(Guid id)
	{
		throw new NotImplementedException();
	}

	public Task<ServiceResult<object>> LockUserAsync(Guid id)
	{
		throw new NotImplementedException();
	}

	public Task<ServiceResult<object>> RequestPasswordResetAsync(Guid id)
	{
		throw new NotImplementedException();
	}

	public Task<ServiceResult<object>> ResetPasswordAsync(Guid id, PasswordResetRequest request)
	{
		throw new NotImplementedException();
	}

	public Task<ServiceResult<CursorPaginationResponse<ProfileSummaryResponse>>> SearchUsersAsync(CursorPaginationRequest<ProfileFilterRequest> request)
	{
		throw new NotImplementedException();
	}

	public Task<ServiceResult<object>> UnlockUserAsync(Guid id)
	{
		throw new NotImplementedException();
	}

	public Task<ServiceResult<ProfileResponse>> UpdateUserAsync(Guid id, ProfileUpdateRequest request)
	{
		throw new NotImplementedException();
	}
}
