using JobifyEcom.Contracts.Errors;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;
using JobifyEcom.Enums;
using JobifyEcom.Exceptions;
using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// Provides verification operations for entities such as worker skills.
/// Only administrators should perform these operations.
/// Updates verification status, reviewer info, and comments.
/// </summary>
/// <param name="db">The database context.</param>
/// <param name="appContextService">The application context service.</param>
/// <param name="workerSkillService">The worker skill service.</param>
internal class VerificationService(AppDbContext db, AppContextService appContextService, IWorkerSkillService workerSkillService) : IVerificationService
{
	private readonly AppDbContext _db = db;
	private readonly AppContextService _appContextService = appContextService;
	private readonly IWorkerSkillService _workerSkillService = workerSkillService;

	public async Task<ServiceResult<WorkerSkillResponse>> VerifySkillAsync(Guid skillId, VerifySkillRequest request)
	{
		Verification verification = await _db.Verifications
			.FirstOrDefaultAsync(v => v.EntityId == skillId && v.EntityType == EntityType.Skill)
			?? throw new AppException(ErrorCatalog.VerificationNotFound);

		verification.Status = request.Status;
		verification.ReviewerComment = request.ReviewerComment;
		verification.ReviewerId = _appContextService.GetCurrentUserId();
		verification.ReviewedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();
		return await _workerSkillService.GetSkillByIdAsync(skillId);
	}
}