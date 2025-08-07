using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.Enums;
using JobifyEcom.Models;

namespace JobifyEcom.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly AppDbContext _context;

    public JobApplicationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<JobRequestResponseDto> CreateApplicationAsync(RequestJobDto dto)
    {
        var request = new JobApplication
        {
            Id = Guid.NewGuid(),
            CustomerId = dto.CustomerId,
            JobPostId = dto.JobPostId,
            Status = JobApplicationStatus.Pending,
            DateRequested = DateTime.UtcNow
        };

        _context.JobApplications.Add(request);
        await _context.SaveChangesAsync();

        return new JobRequestResponseDto
        {
            Id = request.Id,
            CustomerId = request.CustomerId,
            JobPostId = request.JobPostId,
            Status = request.Status,
            DateRequested = request.DateRequested
        };
    }

    public async Task<JobRequestResponseDto> GetByIdAsync(Guid id)
    {
        var request = await _context.JobApplications.FindAsync(id);
        if (request == null) return null;

        return new JobRequestResponseDto
        {
            Id = request.Id,
            CustomerId = request.CustomerId,
            JobPostId = request.JobPostId,
            Status = request.Status,
            DateRequested = request.DateRequested
        };
    }

    public async Task<bool> UpdateStatusAsync(Guid id, JobApplicationStatus status)
    {
        var request = await _context.JobApplications.FindAsync(id);
        if (request == null) return false;

        request.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }
}
