using JobifyEcom.Data;
using JobifyEcom.Models;
using JobifyEcom.DTOs;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

public class JobService(AppDbContext db) : IJobService
{
	public async Task<JobPost> CreateJobAsync(Guid userId, CreateJobDto dto)
    {
        var worker = await db.WorkerProfiles.FirstOrDefaultAsync(w => w.UserId == userId);
        if (worker == null) throw new Exception("Worker profile not found");

        var job = new JobPost
        {
            WorkerId = worker.Id,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price
        };

        db.JobPosts.Add(job);
        await db.SaveChangesAsync();
        return job;
    }

    public async Task<List<JobPost>> GetAllJobsAsync()
    {
        return await db.JobPosts.Include(j => j.Worker).ToListAsync();
    }

    public async Task<List<JobPost>> GetJobsByWorkerAsync(Guid workerId)
    {
        return await db.JobPosts
            .Where(j => j.WorkerId == workerId)
            .ToListAsync();
    }
}
