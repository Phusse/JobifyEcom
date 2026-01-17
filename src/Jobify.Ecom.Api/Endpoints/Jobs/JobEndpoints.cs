using Jobify.Ecom.Api.Constants.Routes;
using Jobify.Ecom.Api.Endpoints.Jobs.Handlers;
using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Scalar.AspNetCore;

namespace Jobify.Ecom.Api.Endpoints.Jobs;

internal static class JobEndpoints
{
    public static void MapJobEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder jobsGroup = app.MapGroup(ApiRoutes.Jobs.Base)
            .WithTags("Jobs");

        jobsGroup.MapPost(string.Empty, CreateJobEndpointHandler.Handle)
            .WithName(nameof(CreateJobEndpointHandler))
            .WithSummary("Create a new job")
            .WithDescription("Creates a new job posting for the authenticated user.")
            .Produces<ApiResponse<object>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        jobsGroup.MapGet(ApiRoutes.Jobs.Id, GetJobEndpointHandler.Handle)
            .WithName(nameof(GetJobEndpointHandler))
            .WithSummary("Get a job by ID")
            .WithDescription("Retrieves the details of a specific job.")
            .Produces<ApiResponse<Job>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        jobsGroup.MapPut(ApiRoutes.Jobs.Id, UpdateJobEndpointHandler.Handle)
            .WithName(nameof(UpdateJobEndpointHandler))
            .WithSummary("Update a job")
            .WithDescription("Updates an existing job posting. Only the owner can update it.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
            .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        jobsGroup.MapDelete(ApiRoutes.Jobs.Id, DeleteJobEndpointHandler.Handle)
            .WithName(nameof(DeleteJobEndpointHandler))
            .WithSummary("Delete a job")
            .WithDescription("Deletes an existing job posting. Only the owner can delete it.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
            .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        jobsGroup.MapGet(string.Empty, GetJobsEndpointHandler.Handle)
            .WithName(nameof(GetJobsEndpointHandler))
            .WithSummary("Get all jobs")
            .WithDescription("Retrieves a paginated list of jobs.")
            .Produces<ApiResponse<IEnumerable<Job>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();
    }
}
