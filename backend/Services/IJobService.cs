using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Jobs;

namespace JobifyEcom.Services;

/// <summary>
/// Defines operations for managing <see cref="Models.Job"/> entities,
/// including creation, retrieval, updates, and deletion.
/// <para>
/// All methods return a <see cref="ServiceResult{T}"/> to wrap the response data,
/// along with informational messages or warnings.
/// Exceptions should be used for fatal errors (e.g., not found, unauthorized).
/// </para>
/// </summary>
public interface IJobService
{
    /// <summary>
    /// Creates a new job based on the provided request data.
    /// </summary>
    /// <param name="request">The details of the job to create.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the created <see cref="JobResponse"/>
    /// if successful.
    /// </returns>
    Task<ServiceResult<JobResponse>> CreateJobAsync(JobCreateRequest request);

    /// <summary>
    /// Retrieves a job by its unique identifier.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job to fetch.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the <see cref="JobResponse"/>
    /// if found, or <c>null</c> if the job does not exist.
    /// </returns>
    Task<ServiceResult<JobResponse?>> GetJobByIdAsync(Guid jobId);

    /// <summary>
    /// Updates an existing job with the provided request data.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job to update.</param>
    /// <param name="request">The updated job details.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the updated <see cref="JobResponse"/>
    /// if successful.
    /// </returns>
    Task<ServiceResult<JobResponse>> UpdateJobAsync(Guid jobId, JobUpdateRequest request);

    /// <summary>
    /// Deletes a job by its unique identifier.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job to delete.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing an empty object if successful.
    /// </returns>
    Task<ServiceResult<object>> DeleteJobAsync(Guid jobId);
}
