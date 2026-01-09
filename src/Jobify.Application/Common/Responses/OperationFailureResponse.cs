using Jobify.Application.Models;

namespace Jobify.Application.Common.Responses;

internal record OperationFailureResponse(
	string Id,
	int StatusCode,
	string Title,
	ResponseDetail[] Details
) : BaseOperationResponse<OperationFailureResponse>(Id, Title, Details);
