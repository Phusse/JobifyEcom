using JobifyEcom.Contracts;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;
using JobifyEcom.Enums;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

/// <summary>
/// Handles operations related to worker profiles, such as creation, retrieval, and deletion.
/// </summary>
[Authorize]
[ApiController]
public class WorkerController(IWorkerDomainService workerDomainService) : ControllerBase
{
    private readonly IWorkerService _workerService = workerDomainService.WorkerService;
    private readonly IWorkerSkillService _workerSkillService = workerDomainService.WorkerSkillService;

    /// <summary>
    /// Creates a new worker profile for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// This endpoint allows an authenticated user to create their worker profile.
    /// If the user already has a worker profile, a conflict error is returned.
    /// </remarks>
    /// <returns>
    /// A confirmation message upon successful creation, or an error if the profile already exists.
    /// </returns>
    /// <response code="200">Worker profile created successfully.</response>
    /// <response code="409">A worker profile already exists for this user.</response>
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [HttpPost(ApiRoutes.Worker.Post.CreateProfile)]
    public async Task<IActionResult> CreateProfile()
    {
        ServiceResult<object> result = await _workerService.CreateProfileAsync();
        return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
    }

    /// <summary>
    /// Retrieves the worker profile of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// This endpoint returns the details of the worker profile associated with the authenticated user.
    /// If no profile exists, a not found error is returned.
    /// </remarks>
    /// <returns>The worker profile details.</returns>
    /// <response code="200">Worker profile retrieved successfully.</response>
    /// <response code="404">No worker profile found for this user.</response>
    [ProducesResponseType(typeof(ApiResponse<WorkerProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpGet(ApiRoutes.Worker.Get.Me)]
    public async Task<IActionResult> GetMyProfile()
    {
        ServiceResult<WorkerProfileResponse> result = await _workerService.GetMyProfileAsync();
        return Ok(ApiResponse<WorkerProfileResponse>.Ok(result.Data, result.Message, result.Errors));
    }

    /// <summary>
    /// Deletes the worker profile of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// This endpoint removes the worker profile associated with the authenticated user.
    /// If no profile exists, a not found error is returned.
    /// </remarks>
    /// <returns>A confirmation message upon successful deletion.</returns>
    /// <response code="200">Worker profile deleted successfully.</response>
    /// <response code="404">No worker profile found for this user.</response>
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpDelete(ApiRoutes.Worker.Delete.Profile)]
    public async Task<IActionResult> DeleteProfile()
    {
        ServiceResult<object> result = await _workerService.DeleteProfileAsync();
        return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
    }

    /// <summary>
    /// Adds a new skill to the currently authenticated worker profile.
    /// </summary>
    /// <remarks>
    /// Requires the worker to already have a profile.
    /// The skill will be submitted with associated tags and placed under verification status.
    /// </remarks>
    /// <param name="request">The details of the skill to add, including name, level, experience, and tags.</param>
    /// <returns>The newly created skill with verification status set to <c>Pending</c>.</returns>
    /// <response code="200">Skill successfully added and submitted for verification.</response>
    /// <response code="400">The request is invalid (e.g., missing required fields or tags).</response>
    /// <response code="404">No worker profile exists for the authenticated user.</response>
    [HttpPost(ApiRoutes.Worker.Post.AddSkill)]
    [ProducesResponseType(typeof(ApiResponse<WorkerSkillResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddSkill([FromBody] AddWorkerSkillRequest request)
    {
        ServiceResult<WorkerSkillResponse> result = await _workerSkillService.AddSkillAsync(request);
        return Ok(ApiResponse<WorkerSkillResponse>.Ok(result.Data, result.Message, result.Errors));
    }

    /// <summary>
    /// Removes an existing skill from the currently authenticated worker profile.
    /// </summary>
    /// <remarks>
    /// The skill must belong to the authenticated worker; otherwise, an error is returned.
    /// </remarks>
    /// <param name="skillId">The unique identifier of the skill to be removed.</param>
    /// <returns>A confirmation message upon successful deletion.</returns>
    /// <response code="200">Skill successfully removed.</response>
    /// <response code="404">The skill does not exist or does not belong to the current worker.</response>
    [HttpDelete(ApiRoutes.Worker.Delete.RemoveSkill)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSkill([FromRoute] Guid skillId)
    {
        ServiceResult<object> result = await _workerSkillService.RemoveSkillAsync(skillId);
        return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
    }

    /// <summary>
    /// Retrieves a specific skill by its identifier.
    /// </summary>
    /// <remarks>
    /// This endpoint returns the skill details along with its tags and current verification status.
    /// </remarks>
    /// <param name="workerId">The worker’s unique identifier (not currently used in lookup).</param>
    /// <param name="skillId">The unique identifier of the skill to retrieve.</param>
    /// <returns>The requested skill details.</returns>
    /// <response code="200">Skill retrieved successfully.</response>
    /// <response code="404">The specified skill does not exist.</response>
    [HttpGet(ApiRoutes.Worker.Get.SkillById)]
    [ProducesResponseType(typeof(ApiResponse<WorkerSkillResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSkillById([FromRoute] Guid workerId, [FromRoute] Guid skillId)
    {
        ServiceResult<WorkerSkillResponse> result = await _workerSkillService.GetSkillByIdAsync(skillId);
        return Ok(ApiResponse<WorkerSkillResponse>.Ok(result.Data, result.Message, result.Errors));
    }

    /// <summary>
    /// Verifies a worker’s skill by approving or rejecting it.
    /// </summary>
    /// <remarks>
    /// Only admins (<c>Admin</c> or <c>SuperAdmin</c> roles) can perform this operation.
    /// Verification updates the status of the skill and records reviewer comments.
    /// </remarks>
    /// <param name="workerId">The worker’s unique identifier (not currently used in lookup).</param>
    /// <param name="skillId">The unique identifier of the skill to verify.</param>
    /// <param name="request">The verification decision, including status and reviewer comments.</param>
    /// <returns>The updated skill with its new verification status.</returns>
    /// <response code="200">Skill successfully verified or rejected.</response>
    /// <response code="404">Verification record not found for this skill.</response>
    [Authorize(Roles = $"{nameof(SystemRole.Admin)}, {nameof(SystemRole.SuperAdmin)}")]
    [HttpPost(ApiRoutes.Worker.Post.VerifySkill)]
    [ProducesResponseType(typeof(ApiResponse<WorkerSkillResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifySkill(
        [FromRoute] Guid workerId,
        [FromRoute] Guid skillId,
        [FromBody] VerifySkillRequest request)
    {
        ServiceResult<WorkerSkillResponse> result = await _workerSkillService.VerifySkillAsync(skillId, request);
        return Ok(ApiResponse<WorkerSkillResponse>.Ok(result.Data, result.Message, result.Errors));
    }
}
