using Jobify.Ecom.Domain.Abstractions;
using Jobify.Ecom.Domain.Components.Auditing;
using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Domain.Entities.Jobs;

public class Job : IEntity, IAuditable
{
    internal readonly AuditState AuditState = new();

    private Job() { }

    public Job(Guid postedByUserId, string title, string description, JobType jobType, decimal minSalary, decimal maxSalary, DateTime closingDate)
    {
        PostedByUserId = postedByUserId;

        UpdateTitle(title);
        UpdateDescription(description);
        UpdateJobType(jobType);
        UpdateSalary(minSalary, maxSalary);
        UpdateClosingDate(closingDate);
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public DateTime CreatedAt => AuditState.CreatedAt;
    public DateTime UpdatedAt => AuditState.UpdatedAt;

    public Guid PostedByUserId { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public JobType JobType { get; private set; }

    public decimal MinSalary { get; private set; }
    public decimal MaxSalary { get; private set; }

    public DateTime ClosingDate { get; private set; }

    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("Job title is required.", nameof(newTitle));

        Title = newTitle;
        AuditState.UpdateAudit();
    }

    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
            throw new ArgumentException("Job description is required.", nameof(newDescription));

        Description = newDescription;
        AuditState.UpdateAudit();
    }

    public void UpdateJobType(JobType jobType)
    {
        if (!Enum.IsDefined(jobType))
            throw new ArgumentException("Invalid job type.");

        JobType = jobType;
        AuditState.UpdateAudit();
    }

    public void UpdateSalary(decimal minSalary, decimal maxSalary)
    {
        if (minSalary < 0 || maxSalary < 0 || minSalary > maxSalary)
            throw new ArgumentException("Invalid salary range.");

        MinSalary = minSalary;
        MaxSalary = maxSalary;
        AuditState.UpdateAudit();
    }

    public void UpdateClosingDate(DateTime closingDate)
    {
        if (closingDate <= DateTime.UtcNow)
            throw new ArgumentException("Closing date must be in the future.");

        ClosingDate = closingDate;
        AuditState.UpdateAudit();
    }

    public void Update(
    string? title,
    string? description,
    JobType? jobType,
    decimal? minSalary,
    decimal? maxSalary,
    DateTime? closingDate
)
    {
        // Only update fields that are provided (not null)
        if (title != null) UpdateTitle(title);
        if (description != null) UpdateDescription(description);
        if (jobType.HasValue) UpdateJobType(jobType.Value);
        if (minSalary.HasValue && maxSalary.HasValue) UpdateSalary(minSalary.Value, maxSalary.Value);
        if (closingDate.HasValue) UpdateClosingDate(closingDate.Value);
    }
}
