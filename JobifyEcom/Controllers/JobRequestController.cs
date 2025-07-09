using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JobifyEcom.Services; 
[ApiController]
[Route("api/[controller]")]
public class JobRequestController : ControllerBase
{
    private readonly IJobRequestService _service;

    public JobRequestController(IJobRequestService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> RequestJob([FromBody] RequestJobDto dto)
    {
        var result = await _service.CreateRequestAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var request = await _service.GetByIdAsync(id);
        if (request == null) return NotFound();
        return Ok(request);
    }

    [HttpPut("{id}/accept")]
    public async Task<IActionResult> AcceptRequest(Guid id)
    {
        var success = await _service.UpdateStatusAsync(id, "Accepted");
        return success ? Ok("Request accepted") : NotFound();
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectRequest(Guid id)
    {
        var success = await _service.UpdateStatusAsync(id, "Rejected");
        return success ? Ok("Request rejected") : NotFound();
    }
}