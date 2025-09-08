namespace JobifyEcom.Services;

internal class WorkerDomainService(IWorkerService workers, IWorkerSkillService skills) : IWorkerDomainService
{
	public IWorkerService WorkerService { get; } = workers;

	public IWorkerSkillService WorkerSkillService { get; } = skills;
}