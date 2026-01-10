using Jobify.Application.Common.Responses;

namespace Jobify.Application.Constants.Responses;

internal static partial class ResponseCatalog
{
    public static class Auth
    {
        public static readonly OperationOutcomeResponse RegistrationSuccessful = new(
            Id: "AUTH_REGISTRATION_SUCCESSFUL",
            Title: "User registered successfully.",
            Details: []
        );

        public static readonly OperationFailureResponse EmailAlreadyExists = new(
            Id: "AUTH_EMAIL_ALREADY_EXISTS",
            StatusCode: 409,
            Title: "Email already exists.",
            Details: []
        );
    }
}
