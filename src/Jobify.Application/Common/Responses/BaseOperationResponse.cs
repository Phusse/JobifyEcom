using Jobify.Application.Models;

namespace Jobify.Application.Common.Responses;

internal abstract record BaseOperationResponse<TSelf>(
	string Id,
	string Title,
	ResponseDetail[] Details
) : IOperationResponse
	where TSelf : BaseOperationResponse<TSelf>
{
	protected TSelf Self => (TSelf)this;

	public TSelf WithTitle(string title) => Self with { Title = title };
	public TSelf AppendTitle(string suffix, string separator = " ") => Self with { Title = $"{Title}{separator}{suffix}" };

	public TSelf WithDetails(params ResponseDetail[] details) => Self with { Details = details };
	public TSelf AppendDetails(params ResponseDetail[] additionalDetails)
		=> Self with { Details = [.. Details, .. additionalDetails] };
}
