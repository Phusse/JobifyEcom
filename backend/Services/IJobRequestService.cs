using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Job;
using JobifyEcom.Enums;

namespace JobifyEcom.Services;

/// <summary>
/// Service for managing job applications (create, retrieve, update).
/// </summary>
public interface IJobApplicationService
{
    /// <summary>
    /// Creates a new application for the given job on behalf of the current worker.
    /// </summary>
    /// <param name="jobId">The job to apply for.</param>
    /// <returns>
    /// Result containing the created application, or an error if invalid.
    /// </returns>
    Task<ServiceResult<JobApplicationResponse>> CreateApplicationAsync(Guid jobId);

    /// <summary>
    /// Gets an application by ID, ensuring it belongs to the specified job.
    /// </summary>
    /// <param name="jobId">The parent job ID.</param>
    /// <param name="applicationId">The application ID.</param>
    /// <returns>
    /// Result containing the application if found, or an error if not.
    /// </returns>
    Task<ServiceResult<JobApplicationResponse>> GetByIdAsync(Guid jobId, Guid applicationId);

    /// <summary>
    /// Updates the status of an application, scoped under the given job.
    /// </summary>
    /// <param name="jobId">The parent job ID.</param>
    /// <param name="applicationId">The application ID.</param>
    /// <param name="status">The new status to set.</param>
    /// <returns>
    /// Result indicating success or error (e.g., not found).
    /// </returns>
    Task<ServiceResult<object>> UpdateStatusAsync(Guid jobId, Guid applicationId, JobApplicationStatus status);
}
