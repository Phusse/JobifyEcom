using Jobify.Ecom.Application.Common.Responses;
using Jobify.Ecom.Application.Constants.Http;

namespace Jobify.Ecom.Application.Constants.Responses;

internal static partial class ResponseCatalog
{
     public static class Job
     {
          public static readonly OperationOutcomeResponse JobCreated = new(
              Id: "JOB_CREATED",
              Title: "Job created successfully.",
              Details: []
          );

          public static readonly OperationOutcomeResponse JobFound = new(
               Id: "JOB_FOUND",
               Title: "Job found successfully.",
               Details: []
          );

          public static readonly OperationFailureResponse JobNotFound = new(
               Id: "JOB_NOT_FOUND",
               StatusCode: HttpStatusCodes.NotFound,
               Title: "Job not found.",
               Details: []
          );

          public static readonly OperationOutcomeResponse JobUpdated = new(
               Id: "JOB_UPDATED",
               Title: "Job updated successfully.",
               Details: []
          );

          public static readonly OperationFailureResponse JobModificationForbidden = new(
              Id: "JOB_MODIFICATION_FORBIDDEN",
              StatusCode: HttpStatusCodes.Forbidden,
              Title: "You are not authorized to modify this job.",
              Details: []
          );

          public static readonly OperationOutcomeResponse JobDeleted = new(
               Id: "JOB_DELETED",
               Title: "Job deleted successfully.",
               Details: []
          );

          public static readonly OperationOutcomeResponse JobsRetrieved = new(
               Id: "JOBS_JOBS_RETRIEVED",
               Title: "Jobs retrieved successfully.",
               Details: []
          );
     }
}
