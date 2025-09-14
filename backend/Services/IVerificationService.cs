using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;

namespace JobifyEcom.Services;

/// <summary>
/// Defines operations for verifying entities such as worker skills.
/// Typically used by administrators to update verification status,
/// reviewer information, and comments.
/// </summary>
public interface IVerificationService
{
	/// <summary>
	/// Updates the verification status of a worker's skill (admin-only action).
	/// </summary>
	/// <param name="skillId">The unique identifier of the skill to verify.</param>
	/// <param name="request">The verification request, including status and reviewer comment.</param>
	/// <returns>
	/// A <see cref="ServiceResult{T}"/> containing the updated <see cref="WorkerSkillResponse"/>.
	/// </returns>
	Task<ServiceResult<WorkerSkillResponse>> VerifySkillAsync(Guid skillId, VerifySkillRequest request);
}
