using JobifyEcom.Contracts;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Worker;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

/// <summary>
/// Handles operations related to worker profiles, such as creation, retrieval, and deletion.
/// </summary>
[Authorize]
[ApiController]
public class WorkerController(IWorkerService workerService) : ControllerBase
{
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
        ServiceResult<object> result = await workerService.CreateProfileAsync();
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
    [ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpGet(ApiRoutes.Worker.Get.Me)]
    public async Task<IActionResult> GetMyProfile()
    {
        ServiceResult<ProfileResponse> result = await workerService.GetMyProfileAsync();
        return Ok(ApiResponse<ProfileResponse>.Ok(result.Data, result.Message, result.Errors));
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
        ServiceResult<object> result = await workerService.DeleteProfileAsync();
        return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
    }
}
