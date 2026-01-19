using Jobify.Ecom.Domain.Abstractions;
using Jobify.Ecom.Domain.Components.Auditing;
using Jobify.Ecom.Domain.Entities.Jobs;
using Jobify.Ecom.Domain.Entities.Users;
using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Domain.Entities.JobApplications;

public class JobApplication : IEntity, IAuditable
{
    public readonly AuditState AuditState = new();

    private JobApplication() { }

    public JobApplication(Guid jobId, Guid applicantUserId)
    {
        JobId = jobId;
        ApplicantUserId = applicantUserId;

        AuditState.UpdateAudit();
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public DateTime CreatedAt => AuditState.CreatedAt;
    public DateTime UpdatedAt => AuditState.UpdatedAt;

    public Guid JobId { get; private set; }
    public Job Job { get; private set; } = null!;
    public Guid ApplicantUserId { get; private set; }
    public User ApplicantUser { get; private set; } = null!;

    public JobApplicationStatus Status { get; private set; } = JobApplicationStatus.Submitted;

    public void UpdateStatus(JobApplicationStatus newStatus)
    {
        if (!Enum.IsDefined(newStatus))
            throw new ArgumentException("Invalid job application status.", nameof(newStatus));

        Status = newStatus;
        AuditState.UpdateAudit();
    }
}
