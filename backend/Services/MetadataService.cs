using JobifyEcom.Contracts.Results;
using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Metadata;

namespace JobifyEcom.Services;

/// <summary>
/// Default implementation of <see cref="IMetadataService"/> that provides
/// metadata for enums and, in the future, system-managed lookup tables (e.g., tags).
/// </summary>
/// <param name="enumCache">The singleton enum cache that stores all registered enums.</param>
internal class MetadataService(EnumCache enumCache) : IMetadataService
{
	private readonly EnumCache _enumCache = enumCache;

	public ServiceResult<List<EnumSetResponse>> GetAllEnums()
	{
		List<EnumSetResponse> cachedEnums = [.. _enumCache.GetAll()];
		return ServiceResult<List<EnumSetResponse>>.Create(ResultCatalog.AllEnumsRetrieved, cachedEnums);
	}

	public ServiceResult<EnumSetResponse?> GetEnumByType(string typeName)
	{
		EnumSetResponse? enumSet = _enumCache.GetByTypeName(typeName);
		ServiceResult<EnumSetResponse?> response;

		if (enumSet is null)
		{
			response = ServiceResult<EnumSetResponse?>.Create(ResultCatalog.EnumNotFound.AppendDetails(
				$"You can view all available enums at: '{ApiRoutes.Metadata.Get.AllEnums}'"
			));
		}
		else
		{
			response = ServiceResult<EnumSetResponse?>.Create(ResultCatalog.EnumRetrieved, enumSet);
		}

		return response;
	}
}
