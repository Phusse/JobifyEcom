using JobifyEcom.DTOs;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobifyEcom.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Worker")]
public class WorkerController(IWorkerService workerService) : ControllerBase
{
    [HttpPost("profile")]
    public async Task<IActionResult> CreateProfile(CreateWorkerProfileDto dto)
    {
        try
        {
            var userId = GetUserId();
            var profile = await workerService.CreateProfileAsync(userId, dto);
            return Ok(ApiResponse<object>.Ok(profile, "Worker profile created successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(null, null, [ex.Message]));
        }
    }

    [HttpGet("profiledetails")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = GetUserId();
            var profile = await workerService.GetMyProfileAsync(userId);

            if (profile == null)
                return NotFound(ApiResponse<object>.Fail(null, "No profile found."));

            return Ok(ApiResponse<object>.Ok(profile, "Worker profile retrived."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(null, null, [ex.Message]));
        }
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim)) throw new Exception("Invalid token. User ID not found.");
        return Guid.Parse(claim);
    }
}
