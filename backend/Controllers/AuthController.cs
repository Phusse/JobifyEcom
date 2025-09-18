using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs.Auth;
using JobifyEcom.DTOs;
using JobifyEcom.Services;
using JobifyEcom.Extensions;

namespace JobifyEcom.Controllers;

/// <summary>
/// Handles user authentication operations, including login, token refresh, registration, and logout.
/// Frontend can use these endpoints to manage authentication flows and obtain tokens.
/// </summary>
/// <param name="authService">Service for handling authentication-related operations.</param>
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Authenticates a user and returns JWT tokens upon successful login.
    /// </summary>
    /// <remarks>
    /// Authenticates a user with their <c>email</c> and <c>password</c>.
    /// Upon success, returns two JWT tokens:
    ///
    /// - **Access token**: short-lived token used to authorize API requests.
    /// Include it in the <c>Authorization</c> header for all protected endpoints.
    /// - **Refresh token**: long-lived token used to obtain a new access token without
    /// requiring the user to log in again. Helps maintain user sessions securely.
    ///
    /// Store the access token in memory or a secure storage, and the refresh
    /// token in a secure, http-only cookie or similar safe storage. Tokens must be sent
    /// in the correct header or request body as required by subsequent API calls.
    /// </remarks>
    /// <param name="request">Login credentials including <c>email</c> and <c>password</c>.</param>
    /// <response code="200">Login successful; access and refresh tokens returned.</response>
    /// <response code="400">Invalid request payload or validation failed.</response>
    /// <response code="401">Authentication failed; incorrect email or password.</response>
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [HttpPost(ApiRoutes.Auth.Post.Login)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        ServiceResult<TokenResponse> result = await _authService.LoginAsync(request);
        return Ok(result.MapToApiResponse());
    }

    /// <summary>
    /// Issues a new access token using a valid refresh token.
    /// </summary>
    /// <remarks>
    /// Allows a client to obtain a fresh access token without requiring the user to log in again.
    /// The provided refresh token must be valid, not expired, and not revoked.
    ///
    /// Tokens should be stored securely. The new access token is used to authorize requests to protected endpoints,
    /// while the refresh token remains available to request subsequent access tokens as needed.
    /// </remarks>
    /// <param name="request">The refresh token request containing the <c>refreshToken</c> string.</param>
    /// <response code="200">Access token successfully refreshed and returned in the response body.</response>
    /// <response code="400">Invalid request payload or validation failed.</response>
    /// <response code="401">Refresh token is invalid, expired, or revoked.</response>
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [HttpPost(ApiRoutes.Auth.Post.Refresh)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        ServiceResult<TokenResponse> result = await _authService.RefreshTokenAsync(request);
        return Ok(result.MapToApiResponse());
    }

    /// <summary>
    /// Registers a new user account with the provided information.
    /// </summary>
    /// <remarks>
    /// Creates a new user if the specified <c>email</c> is not already in use.
    /// After successful registration, a confirmation link is generated and returned in the <c>Location</c> header.
    /// This link must be visited to activate the account.
    ///
    /// The confirmation link can be used to verify the account in automated flows or during testing.
    /// In production, it is typically sent via email to the user.
    /// </remarks>
    /// <param name="request">User registration details including <c>name</c>, <c>email</c>, and <c>password</c>.</param>
    /// <response code="201">User successfully registered. Confirmation link provided in <c>Location</c> header.</response>
    /// <response code="409">Registration failed because the email is already registered.</response>
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [HttpPost(ApiRoutes.Auth.Post.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        ServiceResult<RegisterResponse> result = await _authService.RegisterAsync(request);

        string? location = result.Data?.ConfirmationLink;
        result.Data = null; // strip data to only included in Location header

        return Created(location, result.MapToApiResponse());
    }

    /// <summary>
    /// Logs out the currently authenticated user by revoking their session and tokens.
    /// </summary>
    /// <remarks>
    /// Requires a valid access token in the <c>Authorization</c> header.
    /// Revokes the user's current session and access tokens, effectively ending the authenticated session.
    /// </remarks>
    /// <response code="200">Logout successful; session and tokens revoked.</response>
    /// <response code="404">User not found or session already terminated.</response>
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [Authorize]
    [HttpPatch(ApiRoutes.Auth.Patch.Logout)]
    public async Task<IActionResult> Logout()
    {
        ServiceResult<object> result = await _authService.LogoutAsync();
        return Ok(result.MapToApiResponse());
    }
}
