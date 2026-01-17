using System.Security.Claims;
using Jobify.Ecom.Api.Constants.Routes;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.CreateJob;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.Jobs.Handlers;

internal static class CreateJobEndpointHandler
{
    public record CreateJobRequest(
        string Title,
        string Description,
        JobType JobType,
        decimal MinSalary,
        decimal MaxSalary,
        DateTime ClosingDate
    );

    public static async Task<IResult> Handle(
        [FromBody] CreateJobRequest request,
        HttpContext httpContext,
        IMediator mediator)
    {
        var userIdString = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return Results.Unauthorized();
        }

        var command = new CreateJobCommand(
            request.Title,
            request.Description,
            request.JobType,
            request.MinSalary,
            request.MaxSalary,
            request.ClosingDate,
            userId
        );

        OperationResult<Guid> result = await mediator.Send(command);

        string location = $"{ApiRoutes.Jobs.Base}/{result.Data}";
        return Results.Created(location, result.WithoutData().ToApiResponse());
    }
}
