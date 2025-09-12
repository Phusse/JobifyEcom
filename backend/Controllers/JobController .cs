using Microsoft.AspNetCore.Mvc;
using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Jobs;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using JobifyEcom.Enums;
using JobifyEcom.Extensions;

namespace JobifyEcom.Controllers;

/// <summary>
/// Handles job and job application operations, including creating, retrieving,
/// updating, and deleting jobs, as well as applying to jobs, retrieving applications,
/// and updating application statuses.
/// </summary>
[Authorize]
[ApiController]
public class JobController(IJobDomainService jobDomain) : ControllerBase
{
    private readonly IJobService _jobService = jobDomain.JobService;
    private readonly IJobApplicationService _jobApplicationService = jobDomain.JobApplicationService;

    /// <summary>
    /// Creates a new job post.
    /// </summary>
    /// <param name="request">The job creation request payload.</param>
    /// <returns>The created job details.</returns>
    /// <response code="201">Job successfully created.</response>
    /// <response code="400">Validation failed or bad request.</response>
    [ProducesResponseType(typeof(ApiResponse<JobResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [HttpPost(ApiRoutes.Job.Post.Create)]
    public async Task<IActionResult> Create([FromBody] JobCreateRequest request)
    {
        ServiceResult<JobResponse> result = await _jobService.CreateJobAsync(request);
        return Created(string.Empty, result.MapToApiResponse());
    }

    /// <summary>
    /// Retrieves a job by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the job.</param>
    /// <returns>The job details.</returns>
    /// <response code="200">Job successfully retrieved.</response>
    /// <response code="404">Job not found.</response>
    [ProducesResponseType(typeof(ApiResponse<JobResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpGet(ApiRoutes.Job.Get.ById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        ServiceResult<JobResponse?> result = await _jobService.GetJobByIdAsync(id);
        return Ok(result.MapToApiResponse());
    }

    /// <summary>
    /// Updates an existing job.
    /// </summary>
    /// <param name="id">The unique identifier of the job to update.</param>
    /// <param name="request">The job update request payload.</param>
    /// <returns>The updated job details.</returns>
    /// <response code="200">Job successfully updated.</response>
    /// <response code="400">Validation failed or bad request.</response>
    /// <response code="404">Job not found.</response>
    [ProducesResponseType(typeof(ApiResponse<JobResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpPatch(ApiRoutes.Job.Patch.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] JobUpdateRequest request)
    {
        ServiceResult<JobResponse> result = await _jobService.UpdateJobAsync(id, request);
        return Ok(result.MapToApiResponse());
    }

    /// <summary>
    /// Deletes a job by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the job.</param>
    /// <returns>A confirmation message upon successful deletion.</returns>
    /// <response code="200">Job successfully deleted.</response>
    /// <response code="404">Job not found.</response>
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpDelete(ApiRoutes.Job.Delete.ById)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        ServiceResult<object> result = await _jobService.DeleteJobAsync(id);
        return Ok(result.MapToApiResponse());
    }

    /// <summary>
    /// Submits a new job application for the specified job.
    /// </summary>
    /// <remarks>
    /// Only authenticated users with the <c>Worker</c> role can apply to jobs.
    /// A worker cannot apply to their own job posting or apply multiple times to the same job.
    /// </remarks>
    /// <param name="jobId">The unique identifier of the job to apply for.</param>
    /// <returns>The details of the created job application.</returns>
    /// <response code="201">Application created successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to apply for this job.</response>
    /// <response code="404">Job not found.</response>
    /// <response code="409">Application already exists or user attempted to apply to their own job.</response>
    [ProducesResponseType(typeof(ApiResponse<JobApplicationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [Authorize(Roles = nameof(SystemRole.Worker))]
    [HttpPost(ApiRoutes.Job.Post.Apply)]
    public async Task<IActionResult> Apply([FromRoute] Guid jobId)
    {
        ServiceResult<JobApplicationResponse> result = await _jobApplicationService.CreateApplicationAsync(jobId);
        return Created(string.Empty, result.MapToApiResponse());
    }

    /// <summary>
    /// Retrieves a specific job application.
    /// </summary>
    /// <remarks>
    /// Only the applicant or the job poster is authorized to view the application.
    /// </remarks>
    /// <param name="jobId">The job's unique identifier.</param>
    /// <param name="applicationId">The unique identifier of the application.</param>
    /// <returns>The details of the requested job application.</returns>
    /// <response code="200">Application found and returned successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to view this application.</response>
    /// <response code="404">Application not found.</response>
    [ProducesResponseType(typeof(ApiResponse<JobApplicationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpGet(ApiRoutes.Job.Get.ApplicationById)]
    public async Task<IActionResult> GetApplicationById([FromRoute] Guid jobId, [FromRoute] Guid applicationId)
    {
        ServiceResult<JobApplicationResponse> result = await _jobApplicationService.GetByIdAsync(jobId, applicationId);
        return Ok(result.MapToApiResponse());
    }

    /// <summary>
    /// Accepts a job application.
    /// </summary>
    /// <remarks>
    /// Only the job poster can accept an application.
    /// If the application is already accepted, the request is idempotent and returns success without changes.
    /// </remarks>
    /// <param name="jobId">The job's unique identifier.</param>
    /// <param name="applicationId">The unique identifier of the application.</param>
    /// <returns>Confirmation message.</returns>
    /// <response code="200">Application accepted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to accept this application.</response>
    /// <response code="404">Job or application not found.</response>
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpPatch(ApiRoutes.Job.Patch.AcceptApplication)]
    public async Task<IActionResult> AcceptApplication([FromRoute] Guid jobId, [FromRoute] Guid applicationId)
    {
        ServiceResult<object> result = await _jobApplicationService.UpdateStatusAsync(jobId, applicationId, JobApplicationStatus.Accepted);
        return Ok(result.MapToApiResponse());
    }

    /// <summary>
    /// Accepts a job application.
    /// </summary>
    /// <remarks>
    /// Only the job poster can accept an application.
    /// If the application is already accepted, the request is idempotent and returns success without changes.
    /// </remarks>
    /// <param name="jobId">The job's unique identifier.</param>
    /// <param name="applicationId">The unique identifier of the application.</param>
    /// <returns>Confirmation message.</returns>
    /// <response code="200">Application accepted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to accept this application.</response>
    /// <response code="404">Job or application not found.</response>
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpPatch(ApiRoutes.Job.Patch.RejectApplication)]
    public async Task<IActionResult> RejectApplication([FromRoute] Guid jobId, [FromRoute] Guid applicationId)
    {
        ServiceResult<object> result = await _jobApplicationService.UpdateStatusAsync(jobId, applicationId, JobApplicationStatus.Rejected);
        return Ok(result.MapToApiResponse());
    }
}
