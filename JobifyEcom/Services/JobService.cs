using System.Security.Claims;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Job;
using JobifyEcom.Enums;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;
using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// Service for managing job-related operations.
/// </summary>
internal class JobService(AppDbContext db, IHttpContextAccessor httpContextAccessor) : IJobService
{
    private readonly AppDbContext _db = db;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<ServiceResult<JobResponse>> CreateJobAsync(JobCreateRequest request)
    {
        Guid currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId()
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You need to be signed in to access your account."]
            );

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

        JobResponse response = ToResponse(createdJob);

        return ServiceResult<JobResponse>.Create(response, "Job created successfully.");
    }

    public async Task<ServiceResult<JobResponse?>> GetJobByIdAsync(Guid jobId)
    {
        Job? job = await _db.Jobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new NotFoundException(
                "Job not found.",
                [$"No job exists with the specified ID.({jobId})"]
            );

        JobResponse response = ToResponse(job);

        return ServiceResult<JobResponse?>.Create(response, "Job retrieved successfully.");
    }

    public async Task<ServiceResult<JobResponse>> UpdateJobAsync(Guid jobId, JobUpdateRequest request)
    {
        Job job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new NotFoundException(
                "Job not found.",
                [$"No job exists with the specified ID. ({jobId})"]
            );

        Guid currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId()
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You need to be signed in to access your account."]
            );

        if (job.PostedByUserId != currentUserId)
        {
            throw new ForbiddenException(
                "Access denied.",
                ["You do not have permission to update this job."]
            );
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            job.Title = request.Title.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            job.Description = request.Description.Trim();
        }

        if (request.Price.HasValue)
        {
            job.Price = request.Price.Value;
        }

        if (request.Status.HasValue)
        {
            job.Status = request.Status.Value;
        }

        job.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        JobResponse response = ToResponse(job);

        return ServiceResult<JobResponse>.Create(response, "Job updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteJobAsync(Guid jobId)
    {
        Job? job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new NotFoundException(
                "Job not found.",
                [$"No job exists with the specified ID. ({jobId})"]
            );

        Guid currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId()
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You need to be signed in to access your account."]
            );

        User? currentUser = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId)
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You need to be signed in to access your account."]
            );

        bool isOwner = job.PostedByUserId == currentUserId;
        bool isStaff = currentUser.StaffRole is SystemRole.Admin or SystemRole.SuperAdmin;

        if (!isOwner && !isStaff)
        {
            throw new ForbiddenException(
                "Access denied.",
                ["You do not have permission to delete this job."]
            );
        }

        _db.Jobs.Remove(job);
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(null, "Job deleted successfully.");
    }

    private static JobResponse ToResponse(Job job) => new()
    {
        Id = job.Id,
        Title = job.Title,
        Description = job.Description,
        Price = job.Price,
        Status = job.Status,
        CreatedAt = job.CreatedAt,
        PostedByUserId = job.PostedByUserId,
    };
}
