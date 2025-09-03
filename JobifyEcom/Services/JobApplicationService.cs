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
/// Handles job application operations: creation, retrieval, and status updates.
/// Ensures applications are linked to the correct job and validates the current worker from context.
/// </summary>
internal class JobApplicationService(AppDbContext context, IHttpContextAccessor httpContextAccessor) : IJobApplicationService
{
    private readonly AppDbContext _db = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<ServiceResult<JobApplicationResponse>> CreateApplicationAsync(Guid jobId)
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

        User? user = await _db.Users.Include(w => w.WorkerProfile)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId)
            ?? throw new NotFoundException(
                "Account not found.",
                ["We couldn't find your account. Please contact support if this issue continues."]
            );

        if (user.WorkerProfile is null)
        {
            throw new UnauthorizedException("You must have a worker profile to apply for jobs.");
        }

        Job job = await _db.Jobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new NotFoundException($"The job with ID '{jobId}' was not found.");

        // Prevent duplicate applications
        bool exists = await _db.JobApplications.AnyAsync(ja => ja.JobPostId == jobId && ja.WorkerId == user.WorkerProfile.Id);

        if (exists) throw new ConflictException("You have already applied to this job.");

        // Prevent self from applying to thier own jobs
        if (user.Id == job.PostedByUserId)
        {
            throw new ConflictException("You cannot apply to your own job.");
        }

        JobApplication newApplication = new()
        {
            WorkerId = user.WorkerProfile.Id,
            JobPostId = jobId,
        };

        _db.JobApplications.Add(newApplication);
        await _db.SaveChangesAsync();

        JobApplicationResponse response = new()
        {
            Id = newApplication.Id,
            WorkerId = newApplication.WorkerId,
            WorkerName = user.Name,
            JobPostId = newApplication.JobPostId,
            JobTitle = job.Title,
            JobPrice = job.Price,
            Status = newApplication.Status,
            DateRequested = newApplication.DateRequested,
        };

        return ServiceResult<JobApplicationResponse>.Create(response, "Application created successfully.");
    }

    public async Task<ServiceResult<JobApplicationResponse>> GetByIdAsync(Guid jobId, Guid applicationId)
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

        // Load the current user with optional worker profile
        User currentUser = await _db.Users
            .Include(u => u.WorkerProfile)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId)
            ?? throw new NotFoundException(
                "Account not found.",
                ["We couldn't find your account. Please contact support if this issue continues."]
            );

        // Load the job (needed for job poster check)
        Job job = await _db.Jobs.AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new NotFoundException($"The job with ID '{jobId}' was not found.");

        // Load the application with Applicant + Applicant.User
        JobApplication application = await _db.JobApplications
            .Include(a => a.Applicant)
            .ThenInclude(w => w.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPostId == jobId)
            ?? throw new NotFoundException($"Application with ID '{applicationId}' not found for job '{jobId}'.");

        // Authorization check
        bool isApplicant = currentUser.WorkerProfile is not null
            && application.WorkerId == currentUser.WorkerProfile.Id;

        bool isJobPoster = job.PostedByUserId == currentUser.Id;

        if (!isApplicant && !isJobPoster)
        {
            throw new ForbiddenException(
                "You are not authorized to view this application.",
                ["Only the applicant or the job poster can access this application."]
            );
        }

        // Build response with the applicant’s actual name (not currentUser)
        JobApplicationResponse response = new()
        {
            Id = application.Id,
            WorkerId = application.WorkerId,
            WorkerName = application.Applicant.User.Name, // ✅ always applicant
            JobPostId = application.JobPostId,
            JobTitle = job.Title,
            JobPrice = job.Price,
            Status = application.Status,
            DateRequested = application.DateRequested,
        };

        return ServiceResult<JobApplicationResponse>.Create(response, "Job application returned.");
    }

    public async Task<ServiceResult<object>> UpdateStatusAsync(Guid jobId, Guid applicationId, JobApplicationStatus status)
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

        // Load job with its poster info
        Job job = await _db.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new NotFoundException($"The job with ID '{jobId}' was not found.");

        // Load application
        JobApplication application = await _db.JobApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPostId == jobId)
            ?? throw new NotFoundException($"Application with ID '{applicationId}' not found for job '{jobId}'.");

        // Authorization: only the job poster can accept/reject
        if (job.PostedByUserId != currentUserId)
        {
            throw new ForbiddenException(
                "You are not authorized to update this application.",
                ["Only the job poster can accept or reject applications."]
            );
        }

        // Idempotency: if status is already the same, just return success
        if (application.Status == status)
        {
            return ServiceResult<object>.Create(null, $"Application is already {status.ToString().ToLower()}.");
        }

        application.Status = status;
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(null, $"Application {status.ToString().ToLower()} successfully.");
    }
}
