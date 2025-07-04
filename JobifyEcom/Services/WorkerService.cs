// Services/WorkerService.cs
using JobifyEcom.Data;
using JobifyEcom.Models;
using JobifyEcom.DTOs;
using Microsoft.EntityFrameworkCore;
using JobifyEcom.Enums;

public class WorkerService : IWorkerService
{
    private readonly AppDbContext _db;

    public WorkerService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<WorkerProfile> CreateProfileAsync(Guid userId, CreateWorkerProfileDto dto)
    {
        if (await _db.WorkerProfiles.AnyAsync(w => w.UserId == userId))
            throw new Exception("Profile already exists");

        var profile = new WorkerProfile
        {
            UserId = userId,
            Bio = dto.Bio,
            Skills = dto.Skills
        };

        _db.WorkerProfiles.Add(profile);
        await _db.SaveChangesAsync();
        return profile;
    }

//     public async Task<WorkerProfileDto> GetWorkerByIdAsync(Guid id)
// {
//     var worker = await _db.WorkerProfiles
//         .Include(w => w.Jobs)
//         .FirstOrDefaultAsync(w => w.Id == id);

//     if (worker == null)
//         throw new Exception("Worker not found");

//     var ratings = worker.Jobs
//         .Where(j => j.Rating.HasValue)
//         .Select(j => j.Rating.Value);

//     return new WorkerProfileDto
//     {
//         Id = worker.Id,
//         Name = worker.Name,
//         Email = worker.Email,
//         Skills = worker.Skills,
//         JobsCompleted = worker.Jobs.Count(j => j.Status == JobStatus.Completed),
//         AverageRating = ratings.Any() ? ratings.Average() : null,
//         CreatedAt = worker.CreatedAt
//     };
// }


    public async Task<WorkerProfile?> GetMyProfileAsync(Guid userId)
    {
        return await _db.WorkerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
    }
}
