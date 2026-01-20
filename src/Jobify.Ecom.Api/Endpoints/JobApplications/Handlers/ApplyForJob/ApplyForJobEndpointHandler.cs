using Jobify.Ecom.Api.Constants.Routes;
using Jobify.Ecom.Api.Extensions.Claims;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.JobApplications.ApplyForJob;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.ApplyForJob;

internal static class ApplyForJobEndpointHandler
{
    public static async Task<IResult> Handle([FromBody] ApplyForJobRequest request, HttpContext httpContext, IMediator mediator)
    {
        Guid? internalUserId = httpContext.User.GetInternalUserId();

        ApplyForJobCommand command = new(
            JobId: request.JobId,
            ApplicantUserId: internalUserId
        );

        OperationResult<Guid> result = await mediator.Send(command);

        string location = $"{ApiRoutes.JobApplications.Base}/{result.Data}";
        ApiResponse<object> response = result.WithoutData().ToApiResponse();

        return Results.Created(location, response);
    }
}
