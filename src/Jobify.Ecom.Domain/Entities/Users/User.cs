using Jobify.Ecom.Domain.Abstractions;
using Jobify.Ecom.Domain.Entities.JobApplications;
using Jobify.Ecom.Domain.Entities.Jobs;

namespace Jobify.Ecom.Domain.Entities.Users;

public class User : IEntity
{
    private User() { }

    public User(Guid sourceUserId)
        => SourceUserId = sourceUserId;

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public Guid SourceUserId { get; private set; }

    public IReadOnlyCollection<Job> PostedJobs { get; private set; } = [];
    public IReadOnlyCollection<JobApplication> JobApplications { get; private set; } = [];
}
