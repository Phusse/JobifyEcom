using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Metadata;
using JobifyEcom.Enums;
using JobifyEcom.Extensions;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

/// <summary>
/// Provides endpoints to access system metadata such as enums and lookup tables.
/// </summary>
/// <remarks>
/// Metadata endpoints allow client applications to dynamically retrieve system-managed data
/// used for populating dropdowns, select lists, or performing validation checks.
/// </remarks>
/// <param name="metadataService">Service for retrieving metadata.</param>
[ApiController]
public class MetadataController(IMetadataService metadataService) : ControllerBase
{
	private readonly IMetadataService _metadataService = metadataService;

	/// <summary>
	/// Retrieves all enums registered in the system along with their possible values.
	/// </summary>
	/// <remarks>
	/// Each enum is returned as a set containing its name and associated values.
	/// Useful for populating dropdowns or client-side validation lists.
	/// </remarks>
	/// <returns>A list of <see cref="EnumSetResponse"/> objects.</returns>
	/// <response code="200">Returns a list of all enums in the system.</response>
	[ProducesResponseType(typeof(ApiResponse<List<EnumSetResponse>>), StatusCodes.Status200OK)]
	[HttpGet(ApiRoutes.Metadata.Get.AllEnums)]
	public async Task<IActionResult> GetAllEnums()
	{
		ServiceResult<List<EnumSetResponse>> result = await _metadataService.GetAllEnums();
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Retrieves a specific enum by its type name.
	/// </summary>
	/// <remarks>
	/// Use this endpoint to query for a specific enum by name, e.g., <see cref="SystemRole"/> or <see cref="JobStatus"/>.
	/// The search is case-insensitive. If the enum does not exist, the <c>data</c> field will be <c>null</c>
	/// and an explanatory message will be returned.
	/// </remarks>
	/// <param name="id">The name of the enum type to retrieve (case-insensitive).</param>
	/// <returns>A single <see cref="EnumSetResponse"/> if found; otherwise <c>null</c>.</returns>
	/// <response code="200">Enum found or <c>null</c> if it does not exist.</response>
	[ProducesResponseType(typeof(ApiResponse<EnumSetResponse?>), StatusCodes.Status200OK)]
	[HttpGet(ApiRoutes.Metadata.Get.EnumByType)]
	public async Task<IActionResult> GetEnumByType([FromRoute] string id)
	{
		ServiceResult<EnumSetResponse?> result = await _metadataService.GetEnumByType(id);
		return Ok(result.MapToApiResponse());
	}
}
