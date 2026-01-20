using Jobify.Ecom.Application.Common.Responses;
using Jobify.Ecom.Application.Constants.Http;

namespace Jobify.Ecom.Application.Constants.Responses;

internal static partial class ResponseCatalog
{
    public static class JobApplication
    {
        public static readonly OperationOutcomeResponse Applied = new(
            Id: "JOB_APPLICATION_CREATED",
            Title: "Application submitted successfully.",
            Details: []
        );

        public static readonly OperationOutcomeResponse Found = new(
            Id: "JOB_APPLICATION_FOUND",
            Title: "Application found successfully.",
            Details: []
        );

        public static readonly OperationOutcomeResponse Retrieved = new(
            Id: "JOB_APPLICATIONS_RETRIEVED",
            Title: "Applications retrieved successfully.",
            Details: []
        );

        public static readonly OperationOutcomeResponse StatusUpdated = new(
            Id: "JOB_APPLICATION_STATUS_UPDATED",
            Title: "Application status updated successfully.",
            Details: []
        );

        public static readonly OperationOutcomeResponse Withdrawn = new(
            Id: "JOB_APPLICATION_WITHDRAWN",
            Title: "Application withdrawn successfully.",
            Details: []
        );

        public static readonly OperationFailureResponse NotFound = new(
            Id: "JOB_APPLICATION_NOT_FOUND",
            StatusCode: HttpStatusCodes.NotFound,
            Title: "Application not found.",
            Details: []
        );

        public static readonly OperationFailureResponse AlreadyApplied = new(
            Id: "JOB_APPLICATION_ALREADY_EXISTS",
            StatusCode: HttpStatusCodes.Conflict,
            Title: "You have already applied for this job.",
            Details: []
        );

        public static readonly OperationFailureResponse CannotApplyOwnJob = new(
            Id: "JOB_APPLICATION_OWN_JOB",
            StatusCode: HttpStatusCodes.Forbidden,
            Title: "You cannot apply for your own job.",
            Details: []
        );

        public static readonly OperationFailureResponse JobClosed = new(
            Id: "JOB_APPLICATION_JOB_CLOSED",
            StatusCode: HttpStatusCodes.Forbidden,
            Title: "This job is no longer accepting applications.",
            Details: []
        );

        public static readonly OperationFailureResponse ModificationForbidden = new(
            Id: "JOB_APPLICATION_MODIFICATION_FORBIDDEN",
            StatusCode: HttpStatusCodes.Forbidden,
            Title: "You are not authorized to modify this application.",
            Details: []
        );

        public static readonly OperationFailureResponse InvalidStatusTransition = new(
            Id: "JOB_APPLICATION_INVALID_STATUS_TRANSITION",
            StatusCode: HttpStatusCodes.Conflict,
            Title: "Invalid status transition.",
            Details: []
        );
    }
}
