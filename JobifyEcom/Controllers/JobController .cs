using Microsoft.AspNetCore.Mvc;
using JobifyEcom.Contracts;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Job;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using JobifyEcom.Enums;

namespace JobifyEcom.Controllers;

/// <summary>
/// Handles job-related operations such as creating, retrieving, updating, and deleting jobs.
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
        return Created(string.Empty, ApiResponse<JobResponse>.Ok(result.Data, result.Message, result.Errors));
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
        return Ok(ApiResponse<JobResponse?>.Ok(result.Data, result.Message, result.Errors));
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
        return Ok(ApiResponse<JobResponse>.Ok(result.Data, result.Message, result.Errors));
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
        return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
    }

    [HttpPost(ApiRoutes.Job.Post.Apply)]
    public async Task<IActionResult> Apply([FromRoute] Guid jobId, [FromBody] RequestJobDto dto)
    {
        dto.JobPostId = jobId; // enforce job context
        var result = await _jobApplicationService.CreateApplicationAsync(dto);
        return CreatedAtAction(nameof(GetApplicationById), new { jobId, applicationId = result.Id }, result);
    }

    [HttpGet(ApiRoutes.Job.Get.ApplicationById)]
    public async Task<IActionResult> GetApplicationById([FromRoute] Guid jobId, [FromRoute] Guid applicationId)
    {
        var request = await _jobApplicationService.GetByIdAsync(applicationId);
        if (request == null || request.JobPostId != jobId) return NotFound();
        return Ok(request);
    }

    [HttpPatch(ApiRoutes.Job.Patch.AcceptApplication)]
    public async Task<IActionResult> AcceptApplication([FromRoute] Guid jobId, [FromRoute] Guid applicationId)
    {
        var success = await _jobApplicationService.UpdateStatusAsync(applicationId, JobApplicationStatus.Accepted);
        return success ? Ok("Request accepted") : NotFound();
    }

    [HttpPatch(ApiRoutes.Job.Patch.RejectApplication)]
    public async Task<IActionResult> RejectApplication([FromRoute] Guid jobId, [FromRoute] Guid applicationId)
    {
        var success = await _jobApplicationService.UpdateStatusAsync(applicationId, JobApplicationStatus.Rejected);
        return success ? Ok("Request rejected") : NotFound();
    }
}
