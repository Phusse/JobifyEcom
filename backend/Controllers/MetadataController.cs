using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Metadata;
using JobifyEcom.Extensions;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

/// <summary>
/// Provides endpoints to fetch system metadata such as enums and lookup tables.
/// Frontend can use these endpoints to dynamically populate dropdowns, select lists,
/// and validation rules instead of hardcoding values.
/// </summary>
/// <param name="metadataService">Service for retrieving metadata.</param>
[ApiController]
public class MetadataController(IMetadataService metadataService) : ControllerBase
{
	private readonly IMetadataService _metadataService = metadataService;

	/// <summary>
	/// Retrieves all enums registered in the system along with their possible values.
	/// </summary>
	/// <remarks>
	/// Each enum is returned as an object containing its name and associated values.
	/// Ideal for building dynamic dropdowns or client-side validation lists.
	/// </remarks>
	/// <response code="200">Returns a list of all enums in the system wrapped in ApiResponse.</response>
	[ProducesResponseType(typeof(ApiResponse<List<EnumSetResponse>>), StatusCodes.Status200OK)]
	[HttpGet(ApiRoutes.Metadata.Get.AllEnums)]
	public IActionResult GetAllEnums()
	{
		ServiceResult<List<EnumSetResponse>> result = _metadataService.GetAllEnums();
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Retrieves a specific enum by its type name.
	/// </summary>
	/// <remarks>
	/// Pass the enum name (case-insensitive) to retrieve its values.
	/// Useful when frontend only needs a single enum.
	/// If the enum does not exist, the data field will be null and an explanatory message is returned.
	///
	/// **Valid options:**
	/// - `JobApplicationStatus` : current status of a job application
	/// - `SystemRole` : roles assigned to users in the system
	/// - `SkillLevel` : level of expertise or skill
	/// - `JobStatus` : status of a job posting
	/// - `VerificationStatus` : verification state of an entity
	/// - `UserSortField` : fields available for sorting user lists
	/// - `UserSearchField` : fields that can be searched for filtering users
	/// </remarks>
	/// <param name="id">The name of the enum type to retrieve (case-insensitive), e.g., "SystemRole" or "JobStatus".</param>
	/// <response code="200">Returns the enum if found, or null if it does not exist, wrapped in ApiResponse.</response>
	[ProducesResponseType(typeof(ApiResponse<EnumSetResponse?>), StatusCodes.Status200OK)]
	[HttpGet(ApiRoutes.Metadata.Get.EnumByType)]
	public IActionResult GetEnumByType([FromRoute] string id)
	{
		ServiceResult<EnumSetResponse?> result = _metadataService.GetEnumByType(id);
		return Ok(result.MapToApiResponse());
	}
}
