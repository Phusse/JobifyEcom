namespace JobifyEcom.Services;

/// <summary>
/// Default implementation of <see cref="IJobDomainService"/>.
/// Acts as a facade that exposes job and job application services
/// through a single entry point.
/// </summary>
internal class JobDomainService(IJobService jobs, IJobApplicationService applications) : IJobDomainService
{
	public IJobService JobService { get; } = jobs;

	public IJobApplicationService JobApplicationService { get; } = applications;
}
