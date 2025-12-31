using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Common.Responses;

internal interface IOperationResponse
{
    public string Id { get; }
    public string Title { get; }
    public ResponseDetail[] Details { get; }
}
