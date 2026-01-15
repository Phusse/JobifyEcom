using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Common.Responses;

internal record OperationOutcomeResponse(
	string Id,
	string Title,
	ResponseDetail[] Details,
	object? Data = default
) : BaseOperationResponse<OperationOutcomeResponse>(Id, Title, Details)
{
	public OperationOutcomeResponse<T> As<T>() => new(Id, Title, Details);
}

internal record OperationOutcomeResponse<TData>(
	string Id,
	string Title,
	ResponseDetail[] Details,
	TData? Data = default
) : BaseOperationResponse<OperationOutcomeResponse<TData>>(Id, Title, Details)
{
	public OperationOutcomeResponse<TData> WithData(TData data) => this with { Data = data };
}
