using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;
using JobifyEcom.Extensions;
using JobifyEcom.Helpers;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

/// <summary>
/// Provides endpoints to manage worker profiles and skills, including creation, retrieval, deletion,
/// and skill verification.
/// </summary>
/// <param name="workerService">Service for handling worker profile operations.</param>
/// <param name="workerSkillService">Service for managing worker skills and verification.</param>
[Authorize]
[ApiController]
public class WorkerController(IWorkerService workerService, IWorkerSkillService workerSkillService) : ControllerBase
{
    private readonly IWorkerService _workerService = workerService;
    private readonly IWorkerSkillService _workerSkillService = workerSkillService;

    /// <summary>
    /// Creates a new worker profile for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// This endpoint allows an authenticated user to create their worker profile.
    /// If a profile already exists, the request will fail with a conflict error.
    /// The profile stores information that may be used for job applications, skill tracking,
    /// and other worker-related operations.
    /// </remarks>
    /// <returns>A confirmation message indicating success or failure.</returns>
    /// <response code="201">Worker profile created successfully.</response>
    /// <response code="409">A worker profile already exists for this user.</response>
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [HttpPost(ApiRoutes.Worker.Post.Create)]
    public async Task<IActionResult> CreateProfile()
    {
        ServiceResult<object> result = await _workerService.CreateProfileAsync();
        string locationUri = ResourceUrlBuilder.BuildWorkerProfileResourceUrl(Request);
        return Created(locationUri, result.MapToApiResponse());
    }

    /// <summary>
    /// Retrieves the worker profile of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Returns detailed information about the worker profile associated with the authenticated user.
    /// This includes personal information, skills, and other profile-related data that can be used
    /// for job applications, skill verification, and internal tracking.
    /// If the user does not have a profile, a not found error is returned.
    /// </remarks>
    /// <returns>The full worker profile details for the authenticated user.</returns>
    /// <response code="200">Worker profile retrieved successfully.</response>
    /// <response code="404">No worker profile exists for the authenticated user.</response>
    [ProducesResponseType(typeof(ApiResponse<WorkerProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpGet(ApiRoutes.Worker.Get.Me)]
    public async Task<IActionResult> GetMyProfile()
    {
        ServiceResult<WorkerProfileResponse> result = await _workerService.GetMyProfileAsync();
        return Ok(result.MapToApiResponse());
    }

    /// <summary>
    /// Deletes the worker profile of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Removes all data associated with the authenticated user's worker profile,
    /// including personal details, skills, and any related information.
    /// This operation is irreversible. If the user does not have a worker profile,
    /// a not found error is returned.
    /// </remarks>
    /// <returns>A confirmation message indicating the profile has been deleted.</returns>
    /// <response code="200">Worker profile deleted successfully.</response>
    /// <response code="404">No worker profile exists for the authenticated user.</response>
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpDelete(ApiRoutes.Worker.Delete.Me)]
    public async Task<IActionResult> DeleteProfile()
    {
        ServiceResult<object> result = await _workerService.DeleteProfileAsync();
        return Ok(result.MapToApiResponse());
    }

    /// <summary>
    /// Adds a new skill to the currently authenticated worker profile.
    /// </summary>
    /// <remarks>
    /// The worker must already have a profile to add a skill.
    /// Each new skill is submitted with its associated tags and is placed under a verification workflow,
    /// ensuring that only approved skills are recognized in the system.
    /// If required fields or tags are missing, the request will be rejected.
    /// </remarks>
    /// <param name="request">Details of the skill to add, including <c>name</c>, <c>level</c>, <c>experience</c>, and <c>tags</c>.</param>
    /// <returns>The newly created skill, including its verification status set to <c>Pending</c>.</returns>
    /// <response code="200">Skill successfully added and submitted for verification.</response>
    /// <response code="400">Invalid request due to missing or malformed fields.</response>
    /// <response code="404">The authenticated user does not have a worker profile.</response>
    [ProducesResponseType(typeof(ApiResponse<WorkerSkillResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpPost(ApiRoutes.Worker.Post.AddSkill)]
    public async Task<IActionResult> AddSkill([FromBody] AddWorkerSkillRequest request)
    {
        ServiceResult<WorkerSkillResponse> result = await _workerSkillService.AddSkillAsync(request);
        string locationUri = ResourceUrlBuilder.BuildSkillResourceUrl(Request, result.Data);
        return Created(locationUri, result.MapToApiResponse());
    }

    /// <summary>
    /// Removes an existing skill from the currently authenticated worker profile.
    /// </summary>
    /// <remarks>
    /// The skill must belong to the authenticated worker.
    /// If the skill does not exist, a not found error is returned.
    /// This endpoint ensures that a worker can only remove skills from their own profile.
    /// </remarks>
    /// <param name="skillId">The unique identifier of the skill to remove.</param>
    /// <returns>A confirmation message upon successful deletion.</returns>
    /// <response code="200">Skill successfully removed.</response>
    /// <response code="404">The skill does not exist in the authenticated user's profile.</response>
    [HttpDelete(ApiRoutes.Worker.Delete.RemoveSkill)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSkill([FromRoute] Guid skillId)
    {
        ServiceResult<object> result = await _workerSkillService.RemoveSkillAsync(skillId);
        return Ok(result.MapToApiResponse());
    }

    /// <summary>
    /// Retrieves a specific skill from the currently authenticated worker profile.
    /// </summary>
    /// <remarks>
    /// Returns the skill details along with its tags and current verification status.
    /// The skill must belong to the authenticated user.
    /// </remarks>
    /// <param name="skillId">The unique identifier of the skill to retrieve.</param>
    /// <returns>The requested skill details.</returns>
    /// <response code="200">Skill retrieved successfully.</response>
    /// <response code="404">The specified skill does not exist or does not belong to the current user.</response>
    [ProducesResponseType(typeof(ApiResponse<WorkerSkillResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpGet(ApiRoutes.Worker.Get.SkillById)]
    public async Task<IActionResult> GetSkillById([FromRoute] Guid skillId)
    {
        ServiceResult<WorkerSkillResponse> result = await _workerSkillService.GetSkillByIdAsync(skillId);
        return Ok(result.MapToApiResponse());
    }
}
