using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.Enums;
using JobifyEcom.Models;

namespace JobifyEcom.Services;

public class JobApplicationService(AppDbContext context) : IJobApplicationService
{
    public async Task<JobRequestResponseDto> CreateApplicationAsync(RequestJobDto dto)
    {
        var request = new JobApplication
        {
            WorkerId = dto.CustomerId,
            JobPostId = dto.JobPostId,
            Status = JobApplicationStatus.Pending,
        };

        context.JobApplications.Add(request);
        await context.SaveChangesAsync();

        return new JobRequestResponseDto
        {
            Id = request.Id,
            CustomerId = request.WorkerId,
            JobPostId = request.JobPostId,
            Status = request.Status,
            DateRequested = request.DateRequested
        };
    }

    public async Task<JobRequestResponseDto> GetByIdAsync(Guid id)
    {
        var request = await context.JobApplications.FindAsync(id);
        if (request == null) return null;

        return new JobRequestResponseDto
        {
            Id = request.Id,
            CustomerId = request.WorkerId,
            JobPostId = request.JobPostId,
            Status = request.Status,
            DateRequested = request.DateRequested
        };
    }

    public async Task<bool> UpdateStatusAsync(Guid id, JobApplicationStatus status)
    {
        var request = await context.JobApplications.FindAsync(id);
        if (request == null) return false;

        request.Status = status;
        await context.SaveChangesAsync();
        return true;
    }
}
