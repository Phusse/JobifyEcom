namespace JobifyEcom.Services;

/// <summary>
/// Aggregates all worker-related services into a single domain entry point.
/// Provides access to worker profile operations and worker skill management.
/// </summary>
public interface IWorkerDomainService
{
	/// <summary>
	/// Service for managing worker profiles (create, retrieve, delete).
	/// </summary>
	IWorkerService WorkerService { get; }

	/// <summary>
	/// Service for managing worker skills (add, remove, verify).
	/// </summary>
	IWorkerSkillService WorkerSkillService { get; }
}
