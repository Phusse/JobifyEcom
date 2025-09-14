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
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
internal class VerificationService(AppDbContext db, IHttpContextAccessor httpContextAccessor) : IVerificationService

{
	private readonly AppDbContext _db = db;
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

	public async Task<ServiceResult<WorkerSkillResponse>> VerifySkillAsync(Guid skillId, VerifySkillRequest request)
	{
		Verification verification = await _db.Verifications
			.FirstOrDefaultAsync(v => v.EntityId == skillId && v.EntityType == EntityType.Skill)
			?? throw new AppException(404, "Verification record not found for this skill.");

		verification.Status = request.Status;
		verification.ReviewerComment = request.ReviewerComment;
		verification.ReviewerId = GetCurrentUserId();
		verification.ReviewedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();
		return await GetSkillByIdAsync(skillId);
	}
}