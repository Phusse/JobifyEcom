using Jobify.Ecom.Api.Constants.Routes;
using Jobify.Ecom.Api.Endpoints.Jobs.Handlers.CreateJob;
using Jobify.Ecom.Api.Endpoints.Jobs.Handlers.DeleteJob;
using Jobify.Ecom.Api.Endpoints.Jobs.Handlers.GetJobById;
using Jobify.Ecom.Api.Endpoints.Jobs.Handlers.GetJobs;
using Jobify.Ecom.Api.Endpoints.Jobs.Handlers.UpdateJob;
using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.Features.Jobs.Models;
using Scalar.AspNetCore;

namespace Jobify.Ecom.Api.Endpoints.Jobs;

internal static class JobEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapJobEndpoints()
        {
            RouteGroupBuilder jobsGroup = app.MapGroup(ApiRoutes.Jobs.Base)
                .WithTags("Jobs");

            jobsGroup.MapPost(string.Empty, CreateJobEndpointHandler.Handle)
                .WithName(nameof(CreateJobEndpointHandler))
                .WithSummary("Create a new job")
                .WithDescription(
                    """
                    Creates a new job posting for the authenticated user.

                    Requirements:
                    - You must be authenticated.
                    - `Title` and `Description` are required and cannot be empty.
                    - `JobType` must be a valid value (FullTime, PartTime, Contract, etc.).
                    - `MinSalary` and `MaxSalary` must be non-negative numbers, and `MaxSalary` must be greater than or equal to `MinSalary`.
                    - `ClosingDate` must be a future date.

                    Behavior:
                    - Only the authenticated user will be recorded as the job owner.
                    - On success, the `Location` header will contain the URI of the newly created job.

                    Possible responses:
                    - `201 Created`: Job successfully created.
                    - `400 Bad Request`: Validation failed (invalid input, missing required fields, or business rules violated).
                    - `401 Unauthorized`: You are not authenticated.
                    - `500 Internal Server Error`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<object>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            jobsGroup.MapGet(ApiRoutes.Jobs.GetById, GetJobByIdEndpointHandler.Handle)
                .WithName(nameof(GetJobByIdEndpointHandler))
                .WithSummary("Retrieve a job by ID")
                .WithDescription(
                    """
                    Retrieves the details of a specific job posting by its unique identifier.

                    Requirements:
                    - The job ID must be a valid GUID.
                    - No authentication is required to view a job.

                    Behavior:
                    - Returns full details of the job, including title, description, job type, salary range, and closing date.
                    - If the job does not exist, a clear error message is returned.

                    Possible responses:
                    - `200 OK`: Job successfully retrieved. Returns a `JobResponse` object containing job details.
                    - `404 Not Found`: No job exists with the provided ID.
                    - `500 Internal Server Error`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<JobResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);

            jobsGroup.MapPut(ApiRoutes.Jobs.GetById, UpdateJobEndpointHandler.Handle)
                .WithName(nameof(UpdateJobEndpointHandler))
                .WithSummary("Update an existing job")
                .WithDescription(
                    """
                    Updates an existing job posting. Only the user who created the job can update it.

                    Requirements:
                    - You must be authenticated.
                    - You must be the owner of the job.
                    - Partial updates are allowed: any combination of title, description, job type, salary, and closing date can be updated.
                    - If provided:
                    - `Title` and `Description` cannot be empty.
                    - `JobType` must be a valid value (FullTime, PartTime, Contract, etc.).
                    - `MinSalary` and `MaxSalary` must be non-negative, and `MaxSalary` must be greater than or equal to `MinSalary`.
                    - `ClosingDate` must be a future date.

                    Behavior:
                    - Only fields included in the request will be updated.
                    - The endpoint enforces ownership: you cannot update a job created by another user.
                    - Validation errors return clear messages describing what is invalid.

                    Possible responses:
                    - `200 OK`: Job successfully updated.
                    - `400 Bad Request`: Validation failed for one or more fields (invalid salary range, past closing date, empty title/description, etc.).
                    - `403 Forbidden`: You are not authorized to update this job.
                    - `404 Not Found`: Job with the provided ID does not exist.
                    - `500 Internal Server Error`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            jobsGroup.MapDelete(ApiRoutes.Jobs.GetById, DeleteJobEndpointHandler.Handle)
                .WithName(nameof(DeleteJobEndpointHandler))
                .WithSummary("Delete a job")
                .WithDescription(
                    """
                    Deletes an existing job posting. Only the user who created the job can delete it.

                    Requirements:
                    - You must be authenticated.
                    - You must be the owner of the job to delete it.

                    Behavior:
                    - Once deleted, the job will be permanently removed from the system.
                    - Attempting to delete a job you do not own will result in a `403 Forbidden` response.
                    - Attempting to delete a job that does not exist will result in a `404 Not Found` response.

                    Possible responses:
                    - `200 OK`: Job successfully deleted.
                    - `403 Forbidden`: You are not authorized to delete this job.
                    - `404 Not Found`: Job with the provided ID does not exist.
                    - `500 Internal Server Error`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            jobsGroup.MapGet(string.Empty, GetJobsEndpointHandler.Handle)
                .WithName(nameof(GetJobsEndpointHandler))
                .WithSummary("Retrieve a list of jobs")
                .WithDescription(
                    """
                    Retrieves a paginated list of job postings, ordered by creation date descending.

                    Query Parameters:
                    - `pageSize` (optional, default 10): Number of jobs to return per page. If provided <= 0, defaults to 10.
                    - `lastCreatedAt` (optional): The creation timestamp of the last job from the previous page (used for cursor-based pagination).
                    - `lastJobId` (optional): The ID of the last job from the previous page (used for cursor-based pagination).

                    Behavior:
                    - Supports cursor-based pagination. Provide both `lastCreatedAt` and `lastJobId` to fetch the next page.
                    - Returns a list of jobs including their title, description, job type, salary range, and closing date.
                    - If no jobs exist, returns an empty list.

                    Possible responses:
                    - `200 OK`: Returns a list of jobs in `JobResponse` format.
                    - `500 Internal Server Error`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<IReadOnlyList<JobResponse>>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);
        }
    }
}
