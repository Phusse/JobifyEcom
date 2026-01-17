using Jobify.Application.Common.Responses;
using Jobify.Application.Constants.Http;

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
            StatusCode: HttpStatusCodes.Conflict,
            Title: "Email already exists.",
            Details: []
        );

        public static readonly OperationOutcomeResponse LoginSuccessful = new(
            Id: "AUTH_LOGIN_SUCCESSFUL",
            Title: "Login successful.",
            Details: []
        );

        public static readonly OperationFailureResponse InvalidCredentials = new(
            Id: "AUTH_INVALID_CREDENTIALS",
            StatusCode: HttpStatusCodes.Unauthorized,
            Title: "Invalid email/username or password.",
            Details: []
        );

        public static readonly OperationOutcomeResponse SessionRefreshed = new(
            Id: "AUTH_SESSION_REFRESHED",
            Title: "Session refreshed successfully.",
            Details: []
        );

        public static readonly OperationOutcomeResponse SessionRevoked = new(
            Id: "AUTH_SESSION_REVOKED",
            Title: "Session revoked successfully.",
            Details: []
        );

        public static readonly OperationOutcomeResponse LogoutSuccessful = new(
            Id: "AUTH_LOGOUT_SUCCESSFUL",
            Title: "Logged out successfully.",
            Details: []
        );

        public static readonly OperationFailureResponse InvalidSession = new(
            Id: "AUTH_INVALID_SESSION",
            StatusCode: HttpStatusCodes.Unauthorized,
            Title: "Session is invalid or expired.",
            Details: []
        );

        public static readonly OperationFailureResponse AccountLocked = new(
            Id: "AUTH_ACCOUNT_LOCKED",
            StatusCode: HttpStatusCodes.Forbidden,
            Title: "Account is locked.",
            Details: []
        );
    }
}
