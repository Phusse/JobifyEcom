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

    public async Task<WorkerProfile?> GetMyProfileAsync(Guid userId)
    {
        return await _db.WorkerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
    }
}
