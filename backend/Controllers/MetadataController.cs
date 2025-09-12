using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Metadata;
using JobifyEcom.Enums;
using JobifyEcom.Extensions;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

/// <summary>
/// Provides access to system metadata such as enums and lookup tables.
/// </summary>
/// <remarks>
/// The metadata endpoints allow client applications to dynamically retrieve
/// enums and other static system-managed data used for things like dropdowns,
/// select lists, and validation rules.
/// </remarks>
/// <param name="metadataService">Service for retrieving metadata.</param>
[ApiController]
public class MetadataController(IMetadataService metadataService) : ControllerBase
{
	private readonly IMetadataService _metadataService = metadataService;

	/// <summary>
	/// Retrieves all enums registered in the system along with their values.
	/// </summary>
	/// <remarks>
	/// Use this endpoint to fetch a list of all enums that the API exposes.
	/// Each enum includes its name and possible values (keys and display names).
	/// </remarks>
	/// <returns>
	/// A list of <see cref="EnumSetResponse"/> objects, where each set
	/// represents a single enum and its associated values.
	/// </returns>
	/// <response code="200">Returns the list of all enums successfully.</response>
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
	/// This endpoint allows you to query for one specific enum by name.
	/// For example, <see cref="SystemRole"/> or <seealso cref="JobStatus"/>.
	///
	/// If the enum does not exist, the response will contain <c>null</c> in the
	/// <c>data</c> field and an explanatory message.
	/// </remarks>
	/// <param name="id">The name of the enum type (case-insensitive).</param>
	/// <returns>
	/// A single <see cref="EnumSetResponse"/> object if found,
	/// or <c>null</c> if the enum does not exist.
	/// </returns>
	/// <response code="200">Enum found and returned successfully.</response>
	[ProducesResponseType(typeof(ApiResponse<EnumSetResponse?>), StatusCodes.Status200OK)]
	[HttpGet(ApiRoutes.Metadata.Get.EnumByType)]
	public async Task<IActionResult> GetEnumByType([FromRoute] string id)
	{
		ServiceResult<EnumSetResponse?> result = await _metadataService.GetEnumByType(id);
		return Ok(result.MapToApiResponse());
	}
}
