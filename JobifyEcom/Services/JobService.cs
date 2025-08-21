using JobifyEcom.Data;
using JobifyEcom.Models;
using JobifyEcom.DTOs;
using Microsoft.EntityFrameworkCore;
using JobifyEcom.Enums;

namespace JobifyEcom.Services;

public class JobService(AppDbContext db) : IJobService
{
    public async Task<JobPost> CreateJobAsync(Guid userId, CreateJobDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new Exception("User profile not found");

        var job = new JobPost
        {
            PostedByUserId = user.Id,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Status = JobStatus.Open,
        };

        db.JobPosts.Add(job);
        await db.SaveChangesAsync();
        return job;
    }

    public async Task<List<JobPost>> GetAllJobsAsync()
    {
        return await db.JobPosts.Include(j => j.PostedBy).ToListAsync();
    }

    public async Task<List<JobPost>> GetJobsByUserAsync(Guid userId)
    {
        return await db.JobPosts
            .Where(j => j.PostedByUserId == userId)
            .ToListAsync();
    }
}
