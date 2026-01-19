using Jobify.Ecom.Application.Common.Responses;
using Jobify.Ecom.Application.Constants.Http;

namespace Jobify.Ecom.Application.Constants.Responses;

internal static partial class ResponseCatalog
{
     public static class Job
     {
          public static readonly OperationOutcomeResponse Created = new(
              Id: "JOB_CREATED",
              Title: "Job created successfully.",
              Details: []
          );

          public static readonly OperationOutcomeResponse Found = new(
               Id: "JOB_FOUND",
               Title: "Job found successfully.",
               Details: []
          );

          public static readonly OperationFailureResponse NotFound = new(
               Id: "JOB_NOT_FOUND",
               StatusCode: HttpStatusCodes.NotFound,
               Title: "Job not found.",
               Details: []
          );

          public static readonly OperationOutcomeResponse Updated = new(
               Id: "JOB_UPDATED",
               Title: "Job updated successfully.",
               Details: []
          );

          public static readonly OperationFailureResponse ModificationForbidden = new(
              Id: "JOB_MODIFICATION_FORBIDDEN",
              StatusCode: HttpStatusCodes.Forbidden,
              Title: "You are not authorized to modify this job.",
              Details: []
          );

          public static readonly OperationFailureResponse InvalidUpdate = new(
               Id: "JOB_INVALID_UPDATE",
               StatusCode: HttpStatusCodes.Conflict,
               Title: "Job update failed due to invalid data.",
               Details: []
          );

          public static readonly OperationOutcomeResponse Deleted = new(
               Id: "JOB_DELETED",
               Title: "Job deleted successfully.",
               Details: []
          );

          public static readonly OperationOutcomeResponse Retrieved = new(
               Id: "JOB_RETRIEVED",
               Title: "Jobs retrieved successfully.",
               Details: []
          );
     }
}
