using JobifyEcom.DTOs.Metadata;
using JobifyEcom.Enums;
using JobifyEcom.Helpers;

namespace JobifyEcom.Services;

/// <summary>
/// A singleton in-memory cache that provides metadata for application enums.
/// </summary>
/// <remarks>
/// <para>
/// This service centralizes all enums that are exposed to API consumers,
/// allowing clients to retrieve enum values dynamically instead of hardcoding them.
/// </para>
/// <para>
/// On construction, it pre-builds <see cref="EnumSetResponse"/> representations for each
/// registered enum type, storing them in a case-insensitive dictionary keyed by type name.
/// </para>
/// <para>
/// Because enums are static and immutable at runtime, this cache is safe to use
/// as a <c>Singleton</c> in dependency injection without concerns of state changes.
/// </para>
/// </remarks>
public class EnumCache
{
	/// <summary>
	/// The dictionary mapping enum type names to their corresponding <see cref="EnumSetResponse"/>.
	/// Keys are case-insensitive.
	/// </summary>
	private readonly Dictionary<string, EnumSetResponse> _cache;

	/// <summary>
	/// Initializes the cache with all supported enums that are exposed via the API.
	/// </summary>
	public EnumCache()
	{
		// Explicitly register enums you want exposed to clients.
		EnumSetResponse[] sets =
		[
			Build<JobApplicationStatus>(),
			Build<SystemRole>(),
			Build<SkillLevel>(),
			Build<JobStatus>(),
			Build<VerificationStatus>(),
			Build<UserSortField>(),
			Build<UserSearchField>(),
		];

		_cache = sets.ToDictionary(s => s.Name, s => s, StringComparer.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Gets all enums with their values and display names.
	/// </summary>
	/// <returns>
	/// A read-only list of <see cref="EnumSetResponse"/> representing all cached enums.
	/// </returns>
	public IReadOnlyList<EnumSetResponse> GetAll() => [.. _cache.Values];

	/// <summary>
	/// Gets a specific enum by its type name (case-insensitive).
	/// </summary>
	/// <param name="typeName">The name of the enum type (e.g., "SystemRole").</param>
	/// <returns>
	/// The <see cref="EnumSetResponse"/> if found; otherwise <c>null</c>.
	/// </returns>
	public EnumSetResponse? GetByTypeName(string typeName) => _cache.TryGetValue(typeName, out var set) ? set : null;

	/// <summary>
	/// Builds an <see cref="EnumSetResponse"/> for the given enum type,
	/// including its values, keys, and display names.
	/// </summary>
	/// <typeparam name="T">The enum type to convert into a metadata set.</typeparam>
	private static EnumSetResponse Build<T>() where T : Enum
	{
		List<EnumOptionResponse> values = [.. Enum.GetValues(typeof(T))
			.Cast<T>()
			.Select(v => new EnumOptionResponse
			{
				Key = v.ToString(),
				DisplayName = EnumDisplayHelper.GetDisplayName(v),
			})];

		return new EnumSetResponse
		{
			Name = typeof(T).Name,
			Values = values,
		};
	}
}
