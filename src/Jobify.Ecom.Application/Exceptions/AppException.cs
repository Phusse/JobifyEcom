using Jobify.Ecom.Application.Common.Responses;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Exceptions;

public class AppException : Exception
{
	public int StatusCode { set; get; }
	public string Id { set; get; }
	public List<ResponseDetail>? Details { set; get; }

	public AppException(string? id, int statusCode, string? title, ResponseDetail[]? details = null)
	: base(ResolveMessage(title))
	{
		Id = string.IsNullOrWhiteSpace(id)
			? "UNKNOWN_ERROR"
			: id.Trim();

		StatusCode = statusCode is > 99 and < 600
			? statusCode
			: 500;

		Details = (details is { Length: > 0 })
			? [.. details]
			: null;
	}

	internal AppException(OperationFailureResponse error) : base(ResolveMessage(error))
	{
		Id = string.IsNullOrWhiteSpace(error.Id)
			? "UNKNOWN_ERROR"
			: error.Id.Trim();

		StatusCode = error.StatusCode is > 99 and < 600
			? error.StatusCode
			: 500;

		Details = (error.Details is { Length: > 0 })
			? [.. error.Details]
			: null;
	}

	private static string ResolveMessage(OperationFailureResponse response)
		=> string.IsNullOrWhiteSpace(response.Title)
			? "An unexpected error occurred."
			: response.Title.Trim();

	private static string ResolveMessage(string? title)
		=> string.IsNullOrWhiteSpace(title)
			? "An unexpected error occurred."
			: title.Trim();
}
