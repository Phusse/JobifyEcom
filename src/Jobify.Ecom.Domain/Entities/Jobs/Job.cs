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
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Job title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Job description is required.", nameof(description));

        if (minSalary < 0 || maxSalary < 0 || minSalary > maxSalary)
            throw new ArgumentException("Invalid salary range.");

        if (closingDate <= DateTime.UtcNow)
            throw new ArgumentException("Closing date must be in the future.");

        PostedByUserId = postedByUserId;
        Title = title;
        Description = description;
        JobType = jobType;
        MinSalary = minSalary;
        MaxSalary = maxSalary;
        ClosingDate = closingDate;
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
}
