using Jobify.Ecom.Api.Constants.Routes;
using Jobify.Ecom.Api.Extensions.Claims;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.CreateJob;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.Jobs.Handlers.CreateJob;

internal static class CreateJobEndpointHandler
{
    public static async Task<IResult> Handle([FromBody] CreateJobRequest request, HttpContext httpContext, IMediator mediator)
    {
        Guid? internalUserId = httpContext.User.GetInternalUserId();

        CreateJobCommand command = new(
            request.Title,
            request.Description,
            request.JobType,
            request.MinSalary,
            request.MaxSalary,
            request.ClosingDate,
            internalUserId
        );

        OperationResult<Guid> result = await mediator.Send(command);

        string location = $"{ApiRoutes.Jobs.Base}/{result.Data}";
        ApiResponse<object> response = result.WithoutData().ToApiResponse();

        return Results.Created(location, response);
    }
}
