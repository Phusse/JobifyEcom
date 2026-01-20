using Jobify.Ecom.Api.Constants.Routes;
using Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.ApplyForJob;
using Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.GetApplicationById;
using Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.GetApplicationsForJob;
using Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.GetMyApplications;
using Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.UpdateApplicationStatus;
using Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.WithdrawApplication;
using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.Features.JobApplications.Models;

namespace Jobify.Ecom.Api.Endpoints.JobApplications;

internal static class JobApplicationEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapJobApplicationEndpoints()
        {
            RouteGroupBuilder applicationsGroup = app.MapGroup(ApiRoutes.JobApplications.Base)
                .WithTags("Job Applications");

            applicationsGroup.MapPost(string.Empty, ApplyForJobEndpointHandler.Handle)
                .WithName(nameof(ApplyForJobEndpointHandler))
                .WithSummary("Apply for a job")
                .WithDescription(
                    """
                    Submit an application for a job posting.

                    Requirements:
                    - You must be authenticated.
                    - The job must exist and still be open (closing date has not passed).
                    - You cannot apply for your own job posting.
                    - You cannot apply for the same job more than once.

                    Behavior:
                    - On success, the `Location` header will contain the URI of the newly created application.
                    - The application status will be set to `Submitted`.

                    Possible responses:
                    - `201 Created`: Application successfully submitted.
                    - `401 Unauthorized`: You are not authenticated.
                    - `403 Forbidden`: Cannot apply for your own job, or job is closed.
                    - `404 Not Found`: Job does not exist.
                    - `409 Conflict`: You have already applied for this job.
                    - `500 Internal Server Error`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<object>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            applicationsGroup.MapGet(ApiRoutes.JobApplications.MyApplications, GetMyApplicationsEndpointHandler.Handle)
                .WithName(nameof(GetMyApplicationsEndpointHandler))
                .WithSummary("Get my job applications")
                .WithDescription(
                    """
                    Retrieves a paginated list of your job applications, ordered by creation date descending.

                    Query Parameters:
                    - `pageSize` (optional, default 10): Number of applications per page.
                    - `lastCreatedAt` (optional): Creation timestamp of the last application from the previous page.
                    - `lastApplicationId` (optional): ID of the last application from the previous page.

                    Behavior:
                    - Supports cursor-based pagination. Provide both `lastCreatedAt` and `lastApplicationId` to fetch the next page.
                    - Returns only your own applications.

                    Possible responses:
                    - `200 OK`: Returns a list of applications.
                    - `401 Unauthorized`: You are not authenticated.
                    - `500 Internal Server Error`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<IReadOnlyList<JobApplicationResponse>>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            applicationsGroup.MapGet(ApiRoutes.JobApplications.GetById, GetApplicationByIdEndpointHandler.Handle)
                .WithName(nameof(GetApplicationByIdEndpointHandler))
                .WithSummary("Get application by ID")
                .WithDescription(
                    """
                    Retrieves detailed information about a specific job application.

                    Requirements:
                    - You must be authenticated.
                    - You must be either the applicant or the job owner to view this application.

                    Possible responses:
                    - `200 OK`: Application details returned.
                    - `401 Unauthorized`: You are not authenticated.
                    - `403 Forbidden`: You are not authorized to view this application.
                    - `404 Not Found`: Application does not exist.
                    - `500 Internal Server Error`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<JobApplicationResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            applicationsGroup.MapPatch(ApiRoutes.JobApplications.GetById, UpdateApplicationStatusEndpointHandler.Handle)
                .WithName(nameof(UpdateApplicationStatusEndpointHandler))
                .WithSummary("Update application status")
                .WithDescription(
                    """
                    Updates the status of a job application. Only the job owner (employer) can update application status.

                    Requirements:
                    - You must be authenticated.
                    - You must be the owner of the job this application is for.
                    - The status transition must be valid:
                      - Submitted → Reviewed or Rejected
                      - Reviewed → Shortlisted or Rejected
                      - Shortlisted → Accepted or Rejected
                      - Accepted/Rejected are terminal states.

                    Possible responses:
                    - `200 OK`: Status successfully updated.
                    - `400 Bad Request`: Invalid status transition.
                    - `401 Unauthorized`: You are not authenticated.
                    - `403 Forbidden`: You are not authorized to update this application.
                    - `404 Not Found`: Application does not exist.
                    - `500 Internal Server Error`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            applicationsGroup.MapDelete(ApiRoutes.JobApplications.GetById, WithdrawApplicationEndpointHandler.Handle)
                .WithName(nameof(WithdrawApplicationEndpointHandler))
                .WithSummary("Withdraw application")
                .WithDescription(
                    """
                    Withdraws (deletes) your job application. Only the applicant can withdraw their own application.

                    Requirements:
                    - You must be authenticated.
                    - You must be the applicant who submitted this application.

                    Behavior:
                    - The application will be permanently deleted.

                    Possible responses:
                    - `200 OK`: Application successfully withdrawn.
                    - `401 Unauthorized`: You are not authenticated.
                    - `403 Forbidden`: You are not authorized to withdraw this application.
                    - `404 Not Found`: Application does not exist.
                    - `500 Internal Server Error`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            app.MapGet(ApiRoutes.JobApplications.ForJob, GetApplicationsForJobEndpointHandler.Handle)
                .WithTags("Job Applications")
                .WithName(nameof(GetApplicationsForJobEndpointHandler))
                .WithSummary("Get applications for a job")
                .WithDescription(
                    """
                    Retrieves a paginated list of applications for a specific job. Only the job owner can access this endpoint.

                    Query Parameters:
                    - `pageSize` (optional, default 10): Number of applications per page.
                    - `lastCreatedAt` (optional): Creation timestamp of the last application from the previous page.
                    - `lastApplicationId` (optional): ID of the last application from the previous page.

                    Requirements:
                    - You must be authenticated.
                    - You must be the owner of the job to view its applications.

                    Possible responses:
                    - `200 OK`: Returns a list of applications.
                    - `401 Unauthorized`: You are not authenticated.
                    - `403 Forbidden`: You are not authorized to view applications for this job.
                    - `404 Not Found`: Job does not exist.
                    - `500 Internal Server Error`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<IReadOnlyList<JobApplicationResponse>>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();
        }
    }
}
