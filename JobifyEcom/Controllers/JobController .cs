using JobifyEcom.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobifyEcom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [Authorize(Roles = "Worker")]
    [HttpPost]
    public async Task<IActionResult> CreateJob(CreateJobDto dto)
    {
        try
        {
            var userId = GetUserId();
            var job = await _jobService.CreateJobAsync(userId, dto);
            return Ok(ApiResponse<object?>.Ok(job, "All jobs fetched successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object?>.Fail(null, null, [ex.Message]));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobs()
    {
        try
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return Ok(ApiResponse<object?>.Ok(jobs, "All jobs fetched successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object?>.Fail(null, null, [ex.Message]));
        }
    }

    [Authorize(Roles = "Worker")]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyJobs()
    {
        try
        {
            var userId = GetUserId();
            var jobs = await _jobService.GetJobsByWorkerAsync(userId);
            return Ok(ApiResponse<object?>.Ok(jobs, "Your job posts"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object?>.Fail(null, null, [ex.Message]));
        }
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim)) throw new Exception("Invalid token. User ID not found.");
        return Guid.Parse(claim);
    }
}
