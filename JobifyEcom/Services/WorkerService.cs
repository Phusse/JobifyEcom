using System.Security.Claims;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Worker;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;
using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// Service for managing worker profiles.
/// </summary>
/// <param name="db">The database context.</param>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
internal class WorkerService(AppDbContext db, IHttpContextAccessor httpContextAccessor) : IWorkerService
{
    private readonly AppDbContext _db = db;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<ServiceResult<object>> CreateProfileAsync()
    {
        ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedException(
                "Authentication required.",
                ["You must be signed in to create a worker profile."]
            );

        Guid currentUserId = currentUserPrincipal.GetUserId()
            ?? throw new UnauthorizedException(
                "Authentication required.",
                ["You must be signed in to create a worker profile."]
            );

        Worker? existingWorker = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == currentUserId);

        if (existingWorker is not null)
        {
            throw new ConflictException(
                "Worker profile already exists.",
                [$"A worker profile with ID {existingWorker.Id} is already associated with this account."]
            );
        }

        Worker newWorker = new()
        {
            UserId = currentUserId,
        };

        _db.Workers.Add(newWorker);
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(null, "Your worker profile has been created successfully.");
    }

    public async Task<ServiceResult<object>> DeleteProfileAsync()
    {
        ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedException(
                "Authentication required.",
                ["You must be signed in to delete a worker profile."]
            );

        Guid currentUserId = currentUserPrincipal.GetUserId()
            ?? throw new UnauthorizedException(
                "Authentication required.",
                ["You must be signed in to delete a worker profile."]
            );

        Worker worker = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == currentUserId)
            ?? throw new NotFoundException(
                "Worker profile not found.",
                ["No worker profile could be found for this user."]
            );

        _db.Workers.Remove(worker);
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(null, "Your worker profile has been deleted successfully.");
    }

    public async Task<ServiceResult<WorkerProfileResponse>> GetMyProfileAsync()
    {
        ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedException(
                "Authentication required.",
                ["You must be signed in to view your worker profile."]
            );

        Guid currentUserId = currentUserPrincipal.GetUserId()
            ?? throw new UnauthorizedException(
                "Authentication required.",
                ["You must be signed in to view your worker profile."]
            );

        Worker worker = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == currentUserId)
            ?? throw new NotFoundException(
                "Worker profile not found.",
                ["No worker profile could be found for this user."]
            );

        WorkerProfileResponse response = new()
        {
            WorkerId = worker.Id,
            UserId = worker.UserId,
            CreatedAt = worker.CreatedAt,
        };

        return ServiceResult<WorkerProfileResponse>.Create(response, "Your worker profile has been retrieved successfully.");
    }
}
