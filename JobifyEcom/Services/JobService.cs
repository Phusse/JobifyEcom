using JobifyEcom.Data;
using JobifyEcom.Models;
using JobifyEcom.DTOs;
using Microsoft.EntityFrameworkCore;

public class JobService : IJobService
{
    private readonly AppDbContext _db;

    public JobService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<JobPost> CreateJobAsync(Guid userId, CreateJobDto dto)
    {
        var worker = await _db.WorkerProfiles.FirstOrDefaultAsync(w => w.UserId == userId);
        if (worker == null) throw new Exception("Worker profile not found");

        var job = new JobPost
        {
            WorkerId = worker.Id,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price
        };

        _db.JobPosts.Add(job);
        await _db.SaveChangesAsync();
        return job;  
    }

    public async Task<List<JobPost>> GetAllJobsAsync()
    {
        return await _db.JobPosts.Include(j => j.Worker).ToListAsync();
    }

    public async Task<List<JobPost>> GetJobsByWorkerAsync(Guid workerId)
    {
        return await _db.JobPosts
            .Where(j => j.WorkerId == workerId)
            .ToListAsync();
    }
}
