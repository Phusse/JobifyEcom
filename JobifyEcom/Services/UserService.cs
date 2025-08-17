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

	public Task<ServiceResult<object>> ConfirmEmailAsync(EmailConfirmRequest request)
	{
		throw new NotImplementedException();
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
