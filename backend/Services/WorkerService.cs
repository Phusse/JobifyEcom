using JobifyEcom.Contracts.Errors;
using JobifyEcom.Contracts.Results;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;
using JobifyEcom.Exceptions;
using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// Provides operations for creating, retrieving, and deleting worker profiles
/// for the currently authenticated user.
/// </summary>
/// <param name="db">The database context used for data access.</param>
/// <param name="appContextService">The application context service.</param>
internal class WorkerService(AppDbContext db, AppContextService appContextService) : IWorkerService
{
    private readonly AppDbContext _db = db;
    private readonly AppContextService _appContextService = appContextService;

    public async Task<ServiceResult<object>> CreateProfileAsync()
    {
        Guid currentUserId = _appContextService.GetCurrentUserId();
        Worker? existingWorker = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == currentUserId);

        if (existingWorker is not null)
        {
            throw new AppException(ErrorCatalog.WorkerProfileExists.AppendDetails(
                $"A worker profile with ID {existingWorker.Id} is already associated with this account."
            ));
        }

        Worker newWorker = new() { UserId = currentUserId };
        _db.Workers.Add(newWorker);
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(ResultCatalog.WorkerProfileCreated);
    }

    public async Task<ServiceResult<object>> DeleteProfileAsync()
    {
        Guid currentUserId = _appContextService.GetCurrentUserId();

        Worker worker = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == currentUserId)
            ?? throw new AppException(ErrorCatalog.WorkerProfileNotFound);

        _db.Workers.Remove(worker);
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(ResultCatalog.WorkerProfileDeleted);
    }

    public async Task<ServiceResult<WorkerProfileResponse>> GetMyProfileAsync()
    {
        Guid currentUserId = _appContextService.GetCurrentUserId();

        Worker worker = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == currentUserId)
            ?? throw new AppException(ErrorCatalog.WorkerProfileNotFound);

        WorkerProfileResponse response = new()
        {
            WorkerId = worker.Id,
            UserId = worker.UserId,
            CreatedAt = worker.CreatedAt,
        };

        return ServiceResult<WorkerProfileResponse>.Create(ResultCatalog.WorkerProfileRetrieved, response);
    }
}
