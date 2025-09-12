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
/// <param name="jobService">Service responsible for managing job-related operations.</param>
/// <param name="jobApplicationService">Service responsible for managing job application operations.</param>
[Authorize]
[ApiController]
public class JobController(IJobService jobService, IJobApplicationService jobApplicationService) : ControllerBase
{
    private readonly IJobService _jobService = jobService;
    private readonly IJobApplicationService _jobApplicationService = jobApplicationService;

    /// <summary>
    /// Creates a new job post in the system.
    /// </summary>
    /// <remarks>
    /// This endpoint allows authenticated users to create a new job posting by providing
    /// details such as title, description, location, salary, and other relevant job information.
    /// Only users with proper authorization can create jobs.
    ///
    /// Upon successful creation, the endpoint returns the job details along with the
    /// URL to access the newly created resource in the <c>Location</c> header.
    /// </remarks>
    /// <param name="request">The job creation request payload containing all necessary job details.</param>
    /// <returns>The details of the created job post.</returns>
    /// <response code="201">Job successfully created. The <c>Location</c> header contains the URL to the new job.</response>
    /// <response code="400">Validation failed or bad request. The payload may be missing required fields or contain invalid data.</response>
    [ProducesResponseType(typeof(ApiResponse<JobResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [HttpPost(ApiRoutes.Job.Post.Create)]
    public async Task<IActionResult> Create([FromBody] JobCreateRequest request)
    {
        ServiceResult<JobResponse> result = await _jobService.CreateJobAsync(request);
        string locationUri = BuildJobResourceUrl(result.Data);
        return Created(locationUri, result.MapToApiResponse());
    }

    /// <summary>
    /// Builds the full URL to access a specific job resource.
    /// </summary>
    /// <param name="data">The response containing the id of the job.</param>
    /// <returns>The fully qualified URL to the newly created job resource.</returns>
    private string BuildJobResourceUrl(JobResponse? data)
    {
        if (data is null) return string.Empty;

        string path = ApiRoutes.Job.Get.ById
            .Replace("{{id}}", data.Id.ToString());

        return $"{Request.Scheme}://{Request.Host}/{path}";
    }

    /// <summary>
    /// Retrieves a job by its unique identifier.
    /// </summary>
    /// <remarks>
    /// This endpoint returns the full details of a job posting identified by <paramref name="id"/>.
    /// It can be used by any authenticated user to view a job's information.
    /// If the job does not exist, a 404 Not Found response is returned.
    /// </remarks>
    /// <param name="id">The unique identifier of the job to retrieve.</param>
    /// <returns>The details of the requested job.</returns>
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
    /// Updates an existing job posting.
    /// </summary>
    /// <remarks>
    /// This endpoint allows the job owner or authorized personnel to update the details of a job posting.
    /// Fields that can be updated include title, description, requirements, pay and so on.
    /// If the job does not exist, a 404 Not Found response is returned.
    /// Validation errors, such as missing required fields or invalid data formats, will result in a 400 Bad Request.
    /// </remarks>
    /// <param name="id">The unique identifier of the job to update.</param>
    /// <param name="request">The job update request payload containing updated information.</param>
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
    /// Deletes a job posting by its unique identifier.
    /// </summary>
    /// <remarks>
    /// This endpoint removes a job from the system permanently.
    /// Only the job owner or authorized personnel can delete the job.
    /// If the job does not exist, a 404 Not Found response is returned.
    /// </remarks>
    /// <param name="id">The unique identifier of the job to delete.</param>
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
    /// A worker cannot apply to their own job posting and cannot submit multiple applications
    /// to the same job. The endpoint validates these conditions and returns appropriate responses.
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
        string locationUri = BuildApplicationResourceUrl(result.Data);
        return Created(locationUri, result.MapToApiResponse());
    }

    /// <summary>
    /// Builds the full URL to access a specific job application resource.
    /// </summary>
    /// <param name="data">The response containing the id of the jobapplication.</param>
    /// <returns>The fully qualified URL to the newly created job application resource.</returns>
    private string BuildApplicationResourceUrl(JobApplicationResponse? data)
    {
        if (data is null) return string.Empty;

        string path = ApiRoutes.Job.Get.ApplicationById
            .Replace("{{jobId}}", data.JobPostId.ToString())
            .Replace("{{applicationId}}", data.Id.ToString());

        return $"{Request.Scheme}://{Request.Host}/{path}";
    }

    /// <summary>
    /// Retrieves a specific job application by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Only the applicant who submitted the application or the user who posted the job
    /// is authorized to view the application. Unauthorized users will receive a 403 response.
    /// </remarks>
    /// <param name="jobId">The unique identifier of the job associated with the application.</param>
    /// <param name="applicationId">The unique identifier of the application to retrieve.</param>
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
    /// Accepts a job application for a specific job.
    /// </summary>
    /// <remarks>
    /// Only the user who posted the job is authorized to accept applications.
    /// If the application has already been accepted, the endpoint is idempotent and will
    /// return success without making any changes.
    /// </remarks>
    /// <param name="jobId">The unique identifier of the job associated with the application.</param>
    /// <param name="applicationId">The unique identifier of the application to accept.</param>
    /// <returns>A confirmation message indicating the acceptance status.</returns>
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
    /// Rejects a job application for a specific job.
    /// </summary>
    /// <remarks>
    /// Only the user who posted the job is authorized to reject applications.
    /// If the application has already been rejected, the endpoint is idempotent and will
    /// return success without making any changes.
    /// </remarks>
    /// <param name="jobId">The unique identifier of the job associated with the application.</param>
    /// <param name="applicationId">The unique identifier of the application to reject.</param>
    /// <returns>A confirmation message indicating the rejection status.</returns>
    /// <response code="200">Application rejected successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to reject this application.</response>
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
