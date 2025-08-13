using JobifyEcom.DTOs;
using JobifyEcom.DTOs.User;

namespace JobifyEcom.Services;

internal class UserService : IUserService
{
	public Task<ServiceResult<object>> ConfirmEmailAsync(EmailConfirmRequest request)
	{
		throw new NotImplementedException();
	}

	public Task<ServiceResult<object>> DeleteUserAsync(Guid id)
	{
		throw new NotImplementedException();
	}

	public Task<ServiceResult<ProfileResponse>> GetCurrentUserAsync()
	{
		throw new NotImplementedException();
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
