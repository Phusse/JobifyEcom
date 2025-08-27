using JobifyEcom.Data;
using JobifyEcom.Models;
using JobifyEcom.DTOs;
using Microsoft.EntityFrameworkCore;
using JobifyEcom.Enums;

namespace JobifyEcom.Services;

public class JobService(AppDbContext db) : IJobService
{
    public async Task<Job> CreateJobAsync(Guid userId, CreateJobDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new Exception("User profile not found");

        var job = new Job
        {
            PostedByUserId = user.Id,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Status = JobStatus.Open,
        };

        db.Jobs.Add(job);
        await db.SaveChangesAsync();
        return job;
    }

    public async Task<List<Job>> GetAllJobsAsync()
    {
        return await db.Jobs.Include(j => j.PostedBy).ToListAsync();
    }

    public async Task<List<Job>> GetJobsByUserAsync(Guid userId)
    {
        return await db.Jobs
            .Where(j => j.PostedByUserId == userId)
            .ToListAsync();
    }
}
