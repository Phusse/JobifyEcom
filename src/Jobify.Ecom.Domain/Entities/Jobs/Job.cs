using Jobify.Ecom.Domain.Abstractions;
using Jobify.Ecom.Domain.Components.Auditing;
using Jobify.Ecom.Domain.Entities.JobApplications;
using Jobify.Ecom.Domain.Entities.Users;
using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Domain.Entities.Jobs;

public class Job : IEntity, IAuditable
{
    public readonly AuditState AuditState = new();

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
    public User PostedByUser { get; private set; } = null!;

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public JobType JobType { get; private set; }

    public decimal MinSalary { get; private set; }
    public decimal MaxSalary { get; private set; }

    public DateTime ClosingDate { get; private set; }

    public IReadOnlyCollection<JobApplication> Applications { get; private set; } = [];

    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("Job title is required.", nameof(newTitle));

        if (newTitle.Length < JobLimits.TitleMinLength || newTitle.Length > JobLimits.TitleMaxLength)
            throw new ArgumentException(
                $"Job title must be between {JobLimits.TitleMinLength} and {JobLimits.TitleMaxLength} characters.",
                nameof(newTitle)
            );

        Title = newTitle;
        AuditState.UpdateAudit();
    }

    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
            throw new ArgumentException("Job description is required.", nameof(newDescription));

        if (newDescription.Length < JobLimits.DescriptionMinLength || newDescription.Length > JobLimits.DescriptionMaxLength)
            throw new ArgumentException(
                $"Job description must be between {JobLimits.DescriptionMinLength} and {JobLimits.DescriptionMaxLength} characters.",
                nameof(newDescription)
            );

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
        if (minSalary < JobLimits.MinAllowedMoney || maxSalary > JobLimits.MaxAllowedMoney || maxSalary < minSalary)
        {
            throw new ArgumentException(
                $"Salary range is invalid. Minimum salary must be at least {JobLimits.MinAllowedMoney}, " +
                $"maximum salary cannot exceed {JobLimits.MaxAllowedMoney}, " +
                "and maximum must be greater than or equal to minimum."
            );
        }

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
}
