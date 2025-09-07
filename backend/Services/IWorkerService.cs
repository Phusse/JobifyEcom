using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;

namespace JobifyEcom.Services;

/// <summary>
/// Defines the contract for worker-related operations.
/// </summary>
public interface IWorkerService
{
    /// <summary>
    /// Create a worker profile for the current authenticated user.
    /// </summary>
    Task<ServiceResult<object>> CreateProfileAsync();

    /// <summary>
    /// Get the worker profile for the current authenticated user.
    /// </summary>
    Task<ServiceResult<WorkerProfileResponse>> GetMyProfileAsync();

    /// <summary>
    /// Delete the worker profile for the given user.
    /// </summary>
    Task<ServiceResult<object>> DeleteProfileAsync();
}
