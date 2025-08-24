using JobifyEcom.Contracts;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Worker;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

[ApiController]
public class WorkerController(IWorkerService workerService) : ControllerBase
{
    [HttpPost(ApiRoutes.Worker.Post.CreateProfile)]
    public async Task<IActionResult> CreateProfile()
    {
        ServiceResult<object> result = await workerService.CreateProfileAsync();
        return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
    }

    [HttpGet(ApiRoutes.Worker.Get.Me)]
    public async Task<IActionResult> GetMyProfile()
    {
        ServiceResult<ProfileResponse> result = await workerService.GetMyProfileAsync();
        return Ok(ApiResponse<ProfileResponse>.Ok(result.Data, result.Message, result.Errors));
    }

    [HttpGet(ApiRoutes.Worker.Get.ById)]
    public async Task<IActionResult> GetProfileById([FromRoute] Guid workerId)
    {
        ServiceResult<ProfileResponse> result = await workerService.GetProfileByIdAsync(workerId);
        return Ok(ApiResponse<ProfileResponse>.Ok(result.Data, result.Message, result.Errors));
    }

    [HttpDelete(ApiRoutes.Worker.Delete.Profile)]
    public async Task<IActionResult> DeleteProfile()
    {
        ServiceResult<object> result = await workerService.DeleteProfileAsync();
        return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
    }
}
