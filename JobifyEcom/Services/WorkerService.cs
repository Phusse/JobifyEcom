using JobifyEcom.Data;
using JobifyEcom.Models;
using JobifyEcom.DTOs;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

public class WorkerService(AppDbContext db) : IWorkerService
{
	public async Task<WorkerProfile> CreateProfileAsync(Guid userId, CreateWorkerProfileDto dto)
    {
        if (await db.WorkerProfiles.AnyAsync(w => w.UserId == userId))
            throw new Exception("Profile already exists");

        var profile = new WorkerProfile
        {
            Name = "remove",
            Email = "remove",
            UserId = userId,
            Bio = dto.Bio,
            // Skills = dto.Skills
        };

        db.WorkerProfiles.Add(profile);
        await db.SaveChangesAsync();
        return profile;
    }

    public async Task<WorkerProfile?> GetMyProfileAsync(Guid userId)
    {
        return await db.WorkerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
    }
}
