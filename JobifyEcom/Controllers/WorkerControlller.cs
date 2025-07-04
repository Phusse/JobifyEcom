using JobifyEcom.DTOs;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JobifyEcom.Common;

namespace JobifyEcom.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Worker")]
public class WorkerController : ControllerBase
{
    private readonly IWorkerService _workerService;

    public WorkerController(IWorkerService workerService)
    {
        _workerService = workerService;
    }

    [HttpPost("profile")]
    public async Task<IActionResult> CreateProfile(CreateWorkerProfileDto dto)
    {
        try
        {
            var userId = GetUserId();
            var profile = await _workerService.CreateProfileAsync(userId, dto);
            return Ok(new ApiResponse<object>(true, "Worker profile created successfully", profile));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(false, ex.Message));
        }
    }

    [HttpGet("profiledetails")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = GetUserId();
            var profile = await _workerService.GetMyProfileAsync(userId);

            if (profile == null)
                return NotFound(new ApiResponse<string>(false, "No profile found"));

            return Ok(new ApiResponse<object>(true, "Worker profile retrieved", profile));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(false, ex.Message));
        }
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim)) throw new Exception("Invalid token. User ID not found.");
        return Guid.Parse(claim);
    }
}
