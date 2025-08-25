using System.Security.Claims;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Worker;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;
using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

internal class WorkerService(AppDbContext db, IHttpContextAccessor httpContextAccessor) : IWorkerService
{
    private readonly AppDbContext _db = db;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<ServiceResult<object>> CreateProfileAsync()
    {
        ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You need to be signed in to access your account."]
            );

        Guid currentUserId = currentUserPrincipal.GetUserId()
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You need to be signed in to access your account."]
            );

        Worker? worker = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == currentUserId);

        if (worker is not null)
        {
            throw new ConflictException("You are alreasy a worker", [$"You already have a worker profile. {worker.Id}"]);
        }

        Worker newWorker = new()
        {
            UserId = currentUserId,
        };

        _db.Workers.Add(newWorker);
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(null, "Worker profile created successfully.");
    }

    public async Task<ServiceResult<object>> DeleteProfileAsync()
    {
        ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You need to be signed in to access your account."]
            );

        Guid currentUserId = currentUserPrincipal.GetUserId()
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You need to be signed in to access your account."]
            );

        Worker worker = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == currentUserId)
            ?? throw new NotFoundException(
                "Worker not found",
                ["Sorry we can find your user account."]
            );

        _db.Workers.Remove(worker);
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(null, "Worker profile deleted successfully.");
    }

    public async Task<ServiceResult<ProfileResponse>> GetMyProfileAsync()
    {
        ClaimsPrincipal currentUserPrincipal = _httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You need to be signed in to access your account."]
            );

        Guid currentUserId = currentUserPrincipal.GetUserId()
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You need to be signed in to access your account."]
            );

        Worker worker = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == currentUserId)
            ?? throw new NotFoundException(
                "Worker not found",
                ["Sorry we can find your user account."]
            );

        ProfileResponse response = new()
        {
            WorkerId = worker.Id,
            UserId = worker.UserId,
            CreatedAt = worker.CreatedAt,
        };

        return ServiceResult<ProfileResponse>.Create(response, "Worker profile retrieved successfully.");
    }
}
