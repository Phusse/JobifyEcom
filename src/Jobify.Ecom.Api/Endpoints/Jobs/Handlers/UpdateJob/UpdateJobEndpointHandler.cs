using Jobify.Ecom.Api.Extensions.Claims;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.UpdateJob;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.Jobs.Handlers.UpdateJob;

internal static class UpdateJobEndpointHandler
{
    public static async Task<IResult> Handle([FromRoute] Guid id, [FromBody] UpdateJobRequest request, HttpContext httpContext, IMediator mediator)
    {
        Guid? internalUserId = httpContext.User.GetInternalUserId();

        UpdateJobCommand command = new(
            JobId: id,
            Title: request.Title,
            Description: request.Description,
            JobType: request.JobType,
            MinSalary: request.MinSalary,
            MaxSalary: request.MaxSalary,
            ClosingDate: request.ClosingDate,
            UpdatedByUserId: internalUserId
        );

        OperationResult<object> result = await mediator.Send(command);

        return Results.Ok(result.ToApiResponse());
    }
}
