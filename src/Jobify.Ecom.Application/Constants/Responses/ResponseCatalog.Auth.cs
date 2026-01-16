using Jobify.Ecom.Application.Common.Responses;
using Jobify.Ecom.Application.Constants.Http;

namespace Jobify.Ecom.Application.Constants.Responses;

internal static partial class ResponseCatalog
{
    public static class Auth
    {
        public static readonly OperationOutcomeResponse RegistrationSuccessful = new(
            Id: "AUTH_REGISTRATION_SUCCESSFUL",
            Title: "User registered successfully.",
            Details: []
        );

        public static readonly OperationFailureResponse InvalidSession = new(
            Id: "AUTH_INVALID_SESSION",
            StatusCode: HttpStatusCodes.Unauthorized,
            Title: "Session is invalid or expired.",
            Details: []
        );

        public static readonly OperationFailureResponse UserAlreadyExists = new(
            Id: "AUTH_USER_ALREADY_EXISTS",
            StatusCode: HttpStatusCodes.Conflict,
            Title: "User already exists.",
            Details: []
        );
    }
}
