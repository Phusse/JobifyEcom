using JobifyEcom.Data;
using JobifyEcom.Models;
using JobifyEcom.DTOs;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

public class WorkerService(AppDbContext db) : IWorkerService
{
    public async Task<Worker> CreateProfileAsync(Guid userId, CreateWorkerProfileDto dto)
    {
        if (await db.Workers.AnyAsync(w => w.UserId == userId))
            throw new Exception("Profile already exists");

        var profile = new Worker();

        db.Workers.Add(profile);
        await db.SaveChangesAsync();
        return profile;
    }

    public async Task<Worker?> GetMyProfileAsync(Guid userId)
    {
        return await db.Workers.FirstOrDefaultAsync(p => p.UserId == userId);
    }
}
