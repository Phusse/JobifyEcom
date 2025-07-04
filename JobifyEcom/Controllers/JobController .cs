using JobifyEcom.DTOs;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JobifyEcom.Common;

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
            return Ok(new ApiResponse<object>(true, "Job created successfully", job));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(false, ex.Message));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobs()
    {
        try
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return Ok(new ApiResponse<object>(true, "All jobs fetched successfully", jobs));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(false, ex.Message));
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
            return Ok(new ApiResponse<object>(true, "Your job posts", jobs));
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
