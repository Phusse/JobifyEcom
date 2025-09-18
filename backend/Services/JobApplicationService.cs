using JobifyEcom.Contracts.Errors;
using JobifyEcom.Contracts.Results;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Jobs;
using JobifyEcom.Enums;
using JobifyEcom.Exceptions;
using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// Handles job application operations: creation, retrieval, and status updates.
/// Ensures applications are linked to the correct job and validates the current worker from context.
/// </summary>
/// <param name="context">The database context.</param>
/// <param name="appContextService">The service for accessing the current user's context.</param>
internal class JobApplicationService(AppDbContext context, AppContextService appContextService) : IJobApplicationService
{
    private readonly AppDbContext _db = context;
    private readonly AppContextService _appContextService = appContextService;

    public async Task<ServiceResult<JobApplicationResponse>> CreateApplicationAsync(Guid jobId)
    {
        User user = await _appContextService.GetCurrentUserAsync();

        if (user.WorkerProfile is null)
            throw new AppException(ErrorCatalog.WorkerProfileMissing);

        Job job = await _db.Jobs.AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new AppException(ErrorCatalog.JobNotFound);

        bool alreadyApplied = await _db.JobApplications
            .AnyAsync(ja => ja.JobPostId == jobId && ja.WorkerId == user.WorkerProfile.Id);

        if (alreadyApplied)
            throw new AppException(ErrorCatalog.AlreadyApplied);

        if (user.Id == job.PostedByUserId)
            throw new AppException(ErrorCatalog.CannotApplyToOwnJob);

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

        return ServiceResult<JobApplicationResponse>.Create(ResultCatalog.JobApplicationCreated, response);
    }

    public async Task<ServiceResult<JobApplicationResponse>> GetByIdAsync(Guid jobId, Guid applicationId)
    {
        User currentUser = await _appContextService.GetCurrentUserAsync();

        Job job = await _db.Jobs.AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new AppException(ErrorCatalog.JobNotFound);

        JobApplication application = await _db.JobApplications
            .Include(a => a.Applicant)
            .ThenInclude(w => w.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPostId == jobId)
            ?? throw new AppException(ErrorCatalog.ApplicationNotFound.WithDetails(
                $"Application with ID '{applicationId}' does not exist for job '{jobId}'."
            ));

        bool isApplicant = currentUser.WorkerProfile is not null && application.WorkerId == currentUser.WorkerProfile.Id;
        bool isJobPoster = job.PostedByUserId == currentUser.Id;

        if (!isApplicant && !isJobPoster)
            throw new AppException(ErrorCatalog.ApplicationForbidden);

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

        return ServiceResult<JobApplicationResponse>.Create(ResultCatalog.JobApplicationRetrieved, response);
    }

    public async Task<ServiceResult<object>> UpdateStatusAsync(Guid jobId, Guid applicationId, JobApplicationStatus status)
    {
        Guid currentUserId = _appContextService.GetCurrentUserId();

        Job job = await _db.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new AppException(ErrorCatalog.JobNotFound);

        JobApplication application = await _db.JobApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPostId == jobId)
            ?? throw new AppException(ErrorCatalog.ApplicationNotFound.WithDetails(
                $"Application with ID '{applicationId}' does not exist for job '{jobId}'."
            ));

        if (job.PostedByUserId != currentUserId)
            throw new AppException(ErrorCatalog.ApplicationStatusForbidden);

        if (application.Status == status)
            return ServiceResult<object>.Create(ResultCatalog.JobApplicationStatusAlreadySet);

        application.Status = status;
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(ResultCatalog.JobApplicationStatusUpdated);
    }
}
