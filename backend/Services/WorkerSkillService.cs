using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;
using JobifyEcom.Enums;
using JobifyEcom.Exceptions;
using JobifyEcom.Extensions;
using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// Handles worker skill management: add, remove, fetch, and verify skills.
/// Ensures authentication, ownership checks, tag handling, and verification flow.
/// </summary>
internal class WorkerSkillService(AppDbContext db, IHttpContextAccessor httpContextAccessor) : IWorkerSkillService
{
	private readonly AppDbContext _db = db;
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

	private Guid GetCurrentUserId()
	{
		return _httpContextAccessor.HttpContext?.User.GetUserId()
			?? throw new AppException(401,
				"Authentication required.",
				["You must be signed in to perform this action."]
			);
	}

	public async Task<ServiceResult<WorkerSkillResponse>> AddSkillAsync(AddWorkerSkillRequest request)
	{
		Guid currentUserId = GetCurrentUserId();

		Worker worker = await _db.Workers
			.FirstOrDefaultAsync(w => w.UserId == currentUserId)
			?? throw new AppException(404,
				"Worker profile not found.",
				["You must create a worker profile before adding skills."]
			);

		if (request.Tags is null || request.Tags.Count == 0)
		{
			throw new AppException(400, "At least one tag is required for a skill.");
		}

		Skill skill = new()
		{
			Name = request.Name,
			Description = request.Description,
			Level = request.Level,
			YearsOfExperience = request.YearsOfExperience,
			CertificationLink = request.CertificationLink,
			WorkerId = worker.Id,
		};

		_db.Skills.Add(skill);

		// Normalize tag names
		List<string> normalizedTags = [.. request.Tags
			.Select(t => t.Trim().ToLowerInvariant())
			.Distinct()];

		// Find existing tags
		List<Tag> existingTags = await _db.Tags
			.Where(t => normalizedTags.Contains(t.Name.ToLower()))
			.ToListAsync();

		// Determine which ones are new
		List<string> existingNames = [.. existingTags.Select(t => t.Name)];
		List<string> newNames = [.. normalizedTags.Except(existingNames)];

		// Add new tags
		List<Tag> newTags = [.. newNames.Select(n => new Tag { Name = n })];
		_db.Tags.AddRange(newTags);

		// Combine all tags
		List<Tag> allTags = [.. existingTags, .. newTags];

		// Add entity-tag relations
		foreach (Tag tag in allTags)
		{
			EntityTag entityTag = new()
			{
				TagId = tag.Id,
				EntityId = skill.Id,
				EntityType = EntityType.Skill,
			};

			_db.EntityTags.Add(entityTag);
		}

		// Add verification
		Verification verification = new()
		{
			EntityType = EntityType.Skill,
			EntityId = skill.Id,
			Status = VerificationStatus.Pending,
		};

		_db.Verifications.Add(verification);
		await _db.SaveChangesAsync();

		WorkerSkillResponse response = new()
		{
			Id = skill.Id,
			WorkerId = worker.Id,
			Name = skill.Name,
			Description = skill.Description,
			Level = skill.Level,
			YearsOfExperience = skill.YearsOfExperience,
			CertificationLink = skill.CertificationLink,
			Tags = [.. allTags.Select(t => t.Name)],
			VerificationStatus = verification.Status,
		};

		return ServiceResult<WorkerSkillResponse>.Create(response, "Skill submitted successfully and is pending verification.");
	}

	public async Task<ServiceResult<object>> RemoveSkillAsync(Guid skillId)
	{
		Guid currentUserId = GetCurrentUserId();

		Worker worker = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == currentUserId)
			?? throw new AppException(404,
				"Worker profile not found.",
				["You must create a worker profile before removing skills."]
			);

		Skill skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == skillId && s.WorkerId == worker.Id)
			?? throw new AppException(404,
				"Skill not found.",
				["This skill does not exist or does not belong to you."]
			);

		_db.Skills.Remove(skill);
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(null, "Skill removed successfully.");
	}

	public async Task<ServiceResult<WorkerSkillResponse>> GetSkillByIdAsync(Guid skillId)
	{
		Skill skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == skillId)
			?? throw new AppException(404, "Skill not found.");

		List<string> tags = await _db.EntityTags
			.Where(et => et.EntityId == skill.Id && et.EntityType == EntityType.Skill)
			.Include(et => et.Tag)
			.Select(et => et.Tag.Name)
			.ToListAsync();

		Verification? verification = await _db.Verifications
			.FirstOrDefaultAsync(v => v.EntityId == skill.Id && v.EntityType == EntityType.Skill);

		WorkerSkillResponse response = new()
		{
			Id = skill.Id,
			WorkerId = skill.WorkerId,
			Name = skill.Name,
			Description = skill.Description,
			Level = skill.Level,
			YearsOfExperience = skill.YearsOfExperience,
			CertificationLink = skill.CertificationLink,
			Tags = tags,
			VerificationStatus = verification?.Status ?? VerificationStatus.Pending,
		};

		return ServiceResult<WorkerSkillResponse>.Create(response, "Skill retrieved successfully.");
	}
}
