using JobifyEcom.Contracts.Errors;
using JobifyEcom.Contracts.Results;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Workers;
using JobifyEcom.Enums;
using JobifyEcom.Exceptions;
using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Services;

/// <summary>
/// Handles worker skill management: add, remove, fetch, and verify skills.
/// Assumes validation of tags is performed by the DTO.
/// </summary>
internal class WorkerSkillService(AppDbContext db, AppContextService appContextService) : IWorkerSkillService
{
	private readonly AppDbContext _db = db;
	private readonly AppContextService _appContextService = appContextService;

	public async Task<ServiceResult<WorkerSkillResponse>> AddSkillAsync(AddWorkerSkillRequest request)
	{
		// Get current user and ensure worker profile exists
		Guid currentUserId = _appContextService.GetCurrentUserId();
		Worker worker = await _db.Workers
			.FirstOrDefaultAsync(w => w.UserId == currentUserId)
			?? throw new AppException(ErrorCatalog.WorkerProfileNotFound.AppendDetails(
				"You need a worker profile to add skills. Create one to continue."
			));

		// Prevent duplicate skill for this worker
		string skillNameNormalized = request.Name.Trim().ToLower();

		bool skillExists = await _db.Skills
			.AnyAsync(s => s.WorkerId == worker.Id && s.Name.ToLower() == skillNameNormalized);

		if (skillExists)
		{
			throw new AppException(ErrorCatalog.SkillAlreadyExists.WithDetails(
				$"The skill '{request.Name}' conflicts with a skill you already have."
			));
		}

		Skill skill = new()
		{
			Name = request.Name.Trim(),
			Description = request.Description?.Trim(),
			Level = request.Level,
			YearsOfExperience = request.YearsOfExperience,
			CertificationLink = request.CertificationLink?.Trim(),
			WorkerId = worker.Id,
		};

		_db.Skills.Add(skill);

		// Normalize and handle tags
		List<string> normalizedTags = [.. request.Tags
			.Select(t => t.Trim())
			.Distinct(StringComparer.OrdinalIgnoreCase)];

		List<Tag> existingTags = await _db.Tags
			.Where(t => normalizedTags.Contains(t.Name.ToLower()))
			.ToListAsync();

		List<string> existingTagNames = [.. existingTags.Select(t => t.Name)];
		List<Tag> newTags = [.. normalizedTags
			.Except(existingTagNames, StringComparer.OrdinalIgnoreCase)
			.Select(n => new Tag { Name = n })];

		_db.Tags.AddRange(newTags);

		List<Tag> allTags = [.. existingTags, .. newTags];

		// Add EntityTag relations
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

		// Create verification record
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

		return ServiceResult<WorkerSkillResponse>.Create(ResultCatalog.SkillAdded, response);
	}

	public async Task<ServiceResult<object>> RemoveSkillAsync(Guid skillId)
	{
		Guid currentUserId = _appContextService.GetCurrentUserId();

		Worker worker = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == currentUserId)
			?? throw new AppException(ErrorCatalog.WorkerProfileNotFound.AppendDetails(
				"You need a worker profile to remove skills. Create one to continue."
			));

		Skill skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == skillId && s.WorkerId == worker.Id)
			?? throw new AppException(ErrorCatalog.SkillNotFound);

		_db.Skills.Remove(skill);
		await _db.SaveChangesAsync();

		return ServiceResult<object>.Create(ResultCatalog.SkillRemoved);
	}

	public async Task<ServiceResult<WorkerSkillResponse>> GetSkillByIdAsync(Guid skillId)
	{
		Skill skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == skillId)
			?? throw new AppException(ErrorCatalog.SkillNotFound);

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

		return ServiceResult<WorkerSkillResponse>.Create(ResultCatalog.SkillRetrieved, response);
	}
}
