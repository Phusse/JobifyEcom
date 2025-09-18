using System.Runtime.CompilerServices;
using JobifyEcom.Contracts.Errors;
using JobifyEcom.Contracts.Results;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Jobs;
using JobifyEcom.Enums;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;
using JobifyEcom.Helpers;
using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// Provides operations for creating, retrieving, updating, and deleting jobs,
/// as well as other job-related actions.
/// </summary>
/// <param name="db">The database context used for data access.</param>
/// <param name="appContextService">The application context service.</param>
internal class JobService(AppDbContext db, AppContextService appContextService) : IJobService
{

    private readonly AppDbContext _db = db;
    private readonly AppContextService _appContextService = appContextService;

    public async Task<ServiceResult<JobResponse>> CreateJobAsync(JobCreateRequest request)
    {
        Guid currentUserId = _appContextService.GetCurrentUserId();

        Job createdJob = new()
        {
            PostedByUserId = currentUserId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            Status = JobStatus.Open,
        };

        _db.Jobs.Add(createdJob);
        await _db.SaveChangesAsync();

        JobResponse response = new()
        {
            Id = createdJob.Id,
            Title = createdJob.Title,
            Description = createdJob.Description,
            Price = createdJob.Price,
            Status = createdJob.Status,
            CreatedAt = createdJob.CreatedAt,
            PostedByUserId = createdJob.PostedByUserId,
        };

        return ServiceResult<JobResponse>.Create(ResultCatalog.JobCreated, response);
    }

    public async Task<ServiceResult<JobResponse?>> GetJobByIdAsync(Guid jobId)
    {
        Job? job = await _db.Jobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new AppException(ErrorCatalog.JobNotFound);

        JobResponse response = new()
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Price = job.Price,
            Status = job.Status,
            CreatedAt = job.CreatedAt,
            PostedByUserId = job.PostedByUserId,
        };

        return ServiceResult<JobResponse?>.Create(ResultCatalog.JobRetrieved, response);
    }

    public async Task<ServiceResult<JobResponse>> UpdateJobAsync(Guid jobId, JobUpdateRequest request)
    {
        Job? job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new AppException(ErrorCatalog.JobNotFound);

        Guid currentUserId = _appContextService.GetCurrentUserId();

        if (job.PostedByUserId != currentUserId)
        {
            throw new AppException(ErrorCatalog.UnauthorizedJobModification);
        }

        bool isUpdated = false;

        isUpdated |= UpdateHelper.TryUpdate(request.Title, v => job.Title = v);
        isUpdated |= UpdateHelper.TryUpdate(request.Description, v => job.Description = v);
        isUpdated |= UpdateHelper.TryUpdate(request.Price, v => job.Price = v);
        isUpdated |= UpdateHelper.TryUpdate(request.Status, v => job.Status = v);

        if (isUpdated)
        {
            job.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        JobResponse response = new()
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Price = job.Price,
            Status = job.Status,
            CreatedAt = job.CreatedAt,
            PostedByUserId = job.PostedByUserId,
        };

        return ServiceResult<JobResponse>.Create(ResultCatalog.JobRetrieved, response);
    }

    public async Task<ServiceResult<object>> DeleteJobAsync(Guid jobId)
    {
        Job? job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new AppException(ErrorCatalog.JobNotFound);

        Guid currentUserId = _appContextService.GetCurrentUserId();
        User? currentUser = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId)
            ?? throw new AppException(ErrorCatalog.AccountNotFound);

        bool isOwner = job.PostedByUserId == currentUserId;
        bool isStaff = currentUser.StaffRole is SystemRole.Admin or SystemRole.SuperAdmin;

        if (!isOwner && !isStaff)
        {
            throw new AppException(ErrorCatalog.UnauthorizedJobDeletion);
        }

        _db.Jobs.Remove(job);
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(ResultCatalog.JobDeleted);
    }
}
