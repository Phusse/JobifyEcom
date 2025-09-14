using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;
using JobifyEcom.Enums;
using JobifyEcom.Extensions;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

/// <summary>
/// Manages verification operations for worker skills. Only admins can perform this operation. Verification updates the status of the data and records reviewer comments.
/// </summary>
/// <param name="workerSkillService">Service for managing worker skills and verification.</param>
[Authorize(Roles = $"{nameof(SystemRole.Admin)}, {nameof(SystemRole.SuperAdmin)}")]
[ApiController]
public class VerificationController(IWorkerSkillService workerSkillService) : ControllerBase
{
	private readonly IWorkerSkillService _workerSkillService = workerSkillService;

	/// <summary>
	/// Verifies a worker’s skill by approving or rejecting it.
	/// </summary>
	/// <remarks>
	/// Only admins (<c>Admin</c> or <c>SuperAdmin</c> roles) can perform this operation.
	/// Verification updates the status of the skill and records reviewer comments.
	/// </remarks>
	/// <param name="id">The unique identifier of the skill to verify.</param>
	/// <param name="request">The verification decision, including status and reviewer comments.</param>
	/// <returns>The updated skill with its new verification status.</returns>
	/// <response code="200">Skill successfully verified or rejected.</response>
	/// <response code="404">Verification record not found for this skill or skill does not belong to the specified worker.</response>
	[ProducesResponseType(typeof(ApiResponse<WorkerSkillResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[HttpPost(ApiRoutes.Verify.Post.VerifySkill)]
	public async Task<IActionResult> VerifySkill([FromRoute] Guid id, [FromBody] VerifySkillRequest request)
	{
		ServiceResult<WorkerSkillResponse> result = await _workerSkillService.VerifySkillAsync(id, request);
		return Ok(result.MapToApiResponse());
	}
}
