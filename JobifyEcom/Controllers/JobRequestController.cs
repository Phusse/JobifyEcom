using Microsoft.AspNetCore.Mvc;
using JobifyEcom.Enums;
using JobifyEcom.Services;
using JobifyEcom.DTOs;

namespace JobifyEcom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobApplicationController(IJobApplicationService service) : ControllerBase
{
	[HttpPost]
    public async Task<IActionResult> RequestJob([FromBody] RequestJobDto dto)
    {
        var result = await service.CreateApplicationAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var request = await service.GetByIdAsync(id);
        if (request == null) return NotFound();
        return Ok(request);
    }

    [HttpPut("{id}/accept")]
    public async Task<IActionResult> AcceptApplication(Guid id)
    {
        var success = await service.UpdateStatusAsync(id, JobApplicationStatus.Accepted);
        return success ? Ok("Request accepted") : NotFound();
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectApplication(Guid id)
    {
        var success = await service.UpdateStatusAsync(id, JobApplicationStatus.Rejected);
        return success ? Ok("Request rejected") : NotFound();
    }
}