using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Jobs;
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
        Guid currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId()
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You need to be signed in to access your account."]
            );

        User user = await _db.Users.Include(w => w.WorkerProfile)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId)
            ?? throw new NotFoundException(
                "Account not found.",
                ["We couldn't find your account. Please contact support if this issue continues."]
            );

        // Invariant: the user must have a WorkerProfile at this point.
        // This should never be null in a consistent system,
        // but we check defensively to guard against data corruption or misconfiguration.
        if (user.WorkerProfile is null)
        {
            throw new UnauthorizedException("Your worker profile is missing. Please complete your profile before applying.");
        }

        Job job = await _db.Jobs.AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new NotFoundException($"No job was found with ID '{jobId}'.");

        // Prevent duplicate applications
        bool alreadyApplied = await _db.JobApplications
            .AnyAsync(ja => ja.JobPostId == jobId && ja.WorkerId == user.WorkerProfile.Id);

        if (alreadyApplied)
        {
            throw new ConflictException("You have already applied for this job.");
        }

        // Prevent users from applying to their own jobs
        if (user.Id == job.PostedByUserId)
        {
            throw new ConflictException("You cannot apply to a job you posted.");
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

        return ServiceResult<JobApplicationResponse>.Create(response, "Your application has been submitted successfully.");
    }

    public async Task<ServiceResult<JobApplicationResponse>> GetByIdAsync(Guid jobId, Guid applicationId)
    {
        Guid currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId()
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["Please sign in to view job applications."]
            );

        // Load the current user with optional worker profile
        User currentUser = await _db.Users
            .Include(u => u.WorkerProfile)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId)
            ?? throw new NotFoundException(
                "Account not found.",
                ["We couldn't find your account. Contact support if the issue persists."]
            );

        // Load the job (needed for job poster check)
        Job job = await _db.Jobs.AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new NotFoundException($"Job with ID '{jobId}' was not found.");

        // Load the application with Applicant + Applicant.User
        JobApplication application = await _db.JobApplications
            .Include(a => a.Applicant)
            .ThenInclude(w => w.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPostId == jobId)
            ?? throw new NotFoundException($"Application with ID '{applicationId}' does not exist for job '{jobId}'.");

        // Authorization check
        bool isApplicant = currentUser.WorkerProfile is not null
            && application.WorkerId == currentUser.WorkerProfile.Id;

        bool isJobPoster = job.PostedByUserId == currentUser.Id;

        if (!isApplicant && !isJobPoster)
        {
            throw new ForbiddenException(
                "Access denied.",
                ["Only the applicant or the job poster can view this application."]
            );
        }

        JobApplicationResponse response = new()
        {
            Id = application.Id,
            WorkerId = application.WorkerId,
            WorkerName = application.Applicant.User.Name,
            JobPostId = application.JobPostId,
            JobTitle = job.Title,
            JobPrice = job.Price,
            Status = application.Status,
            DateRequested = application.DateRequested,
        };

        return ServiceResult<JobApplicationResponse>.Create(response, "Job application retrieved successfully.");
    }

    public async Task<ServiceResult<object>> UpdateStatusAsync(Guid jobId, Guid applicationId, JobApplicationStatus status)
    {
        Guid currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId()
            ?? throw new UnauthorizedException(
                "Sign in required.",
                ["You must be signed in to manage job applications."]
            );

        Job job = await _db.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new NotFoundException($"No job found with ID '{jobId}'.");

        JobApplication application = await _db.JobApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPostId == jobId)
            ?? throw new NotFoundException($"No application found with ID '{applicationId}' for this job.");

        // Authorization: only the job poster can accept/reject
        if (job.PostedByUserId != currentUserId)
        {
            throw new ForbiddenException(
                "Access denied.",
                ["Only the job poster can update the status of applications."]
            );
        }

        // Idempotency: if status is already the same, just return success
        if (application.Status == status)
        {
            return ServiceResult<object>.Create(
                null,
                $"The application is already marked as '{status.ToString().ToLower()}'."
            );
        }

        application.Status = status;
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(
            null,
            $"The application has been successfully marked as '{status.ToString().ToLower()}'."
        );
    }
}
