using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;

namespace JobifyEcom.Services;

/// <summary>
/// Defines operations for managing worker skills.
/// </summary>
public interface IWorkerSkillService
{
	/// <summary>
	/// Adds a new skill to the currently authenticated worker’s profile.
	/// </summary>
	/// <param name="request">The skill details provided by the worker.</param>
	/// <returns>
	/// A <see cref="ServiceResult{T}"/> containing the created <see cref="WorkerSkillResponse"/>.
	/// </returns>
	Task<ServiceResult<WorkerSkillResponse>> AddSkillAsync(AddWorkerSkillRequest request);

	/// <summary>
	/// Removes an existing skill from the currently authenticated worker’s profile.
	/// </summary>
	/// <param name="skillId">The unique identifier of the skill to remove.</param>
	/// <returns>
	/// A <see cref="ServiceResult{T}"/> confirming the deletion operation.
	/// </returns>
	Task<ServiceResult<object>> RemoveSkillAsync(Guid skillId);

	/// <summary>
	/// Retrieves a specific skill belonging to the currently authenticated worker.
	/// </summary>
	/// <param name="skillId">The unique identifier of the skill to retrieve.</param>
	/// <returns>
	/// A <see cref="ServiceResult{T}"/> containing the <see cref="WorkerSkillResponse"/>.
	/// </returns>
	Task<ServiceResult<WorkerSkillResponse>> GetSkillByIdAsync(Guid skillId);
}
