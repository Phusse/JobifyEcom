using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Metadata;

namespace JobifyEcom.Services;

/// <summary>
/// Defines operations for retrieving metadata used across the application,
/// including enums and system-managed lookup tables such as tags.
/// </summary>
/// <remarks>
/// This service abstracts metadata access for the API layer, ensuring that
/// consumers receive consistent, well-structured results. It supports both
/// retrieving all available enums and querying specific enum sets by name.
///
/// <para>Typical usage:
/// <list type="bullet">
///   <item><description>Populate dropdown lists or select options in client applications.</description></item>
///   <item><description>Expose enum definitions via a metadata API endpoint.</description></item>
///   <item><description>Retrieve localized or display-friendly enum values.</description></item>
/// </list>
/// </para>
/// </remarks>
public interface IMetadataService
{
	/// <summary>
	/// Retrieves all enums registered in the system and their associated values.
	/// </summary>
	/// <returns>
	/// A <see cref="ServiceResult{T}"/> containing a list of <see cref="EnumSetResponse"/> objects,
	/// where each <see cref="EnumSetResponse"/> represents one enum and its possible values.
	/// </returns>
	ServiceResult<List<EnumSetResponse>> GetAllEnums();

	/// <summary>
	/// Retrieves a specific enum set by its type name.
	/// </summary>
	/// <param name="typeName">The name of the enum type (e.g., <c>"SystemRole"</c>).</param>
	/// <returns>
	/// A <see cref="ServiceResult{T}"/> containing the <see cref="EnumSetResponse"/> if found;
	/// otherwise, <c>null</c>.
	/// </returns>
	ServiceResult<EnumSetResponse?> GetEnumByType(string typeName);
}
