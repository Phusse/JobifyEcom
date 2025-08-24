using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Worker;

namespace JobifyEcom.Services;

public class WorkerService(AppDbContext db, IHttpContextAccessor httpContextAccessor) : IWorkerService
{
	public Task<ServiceResult<object>> CreateProfileAsync()
	{
		throw new NotImplementedException();
	}

	public Task<ServiceResult<object>> DeleteProfileAsync()
	{
		throw new NotImplementedException();
	}

	public Task<ServiceResult<ProfileResponse>> GetMyProfileAsync()
	{
		throw new NotImplementedException();
	}

	public Task<ServiceResult<ProfileResponse>> GetProfileByIdAsync(Guid workerId)
	{
		throw new NotImplementedException();
	}
}
