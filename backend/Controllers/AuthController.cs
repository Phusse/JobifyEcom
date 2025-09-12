using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs.Auth;
using JobifyEcom.DTOs;
using JobifyEcom.Services;
using JobifyEcom.Extensions;

namespace JobifyEcom.Controllers;

/// <summary>
/// Provides endpoints for user authentication operations, including login, token refresh, registration, and logout.
/// </summary>

[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Authenticates a user and issues JSON Web Tokens (JWT) upon successful login.
    /// </summary>
    /// <remarks>
    /// This endpoint verifies the user's <c>email</c> and <c>password</c>.
    /// Upon successful authentication, it returns:
    /// <list type="bullet">
    ///   <item><description>A short-lived <b>access token</b> (JWT), which must be included in the
    ///   <c>Authorization</c> header of subsequent requests to access protected resources.</description></item>
    ///   <item><description>A long-lived <b>refresh token</b> (JWT), which can be used to obtain new access tokens
    ///   without requiring the user to re-enter credentials.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="request">The login credentials, including <c>email</c> and <c>password</c>.</param>
    /// <returns>A response containing both access and refresh tokens along with their expiration times.</returns>
    /// <response code="200">Login successful. Tokens are returned in the response body.</response>
    /// <response code="400">Invalid login payload or failed validation.</response>
    /// <response code="401">Authentication failed due to incorrect credentials.</response>
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
    /// This endpoint allows a client to obtain a fresh access token without re-authenticating.
    /// The provided refresh token must be valid, not expired, and not revoked.
    /// </remarks>
    /// <param name="request">The refresh token request containing the <c>refreshToken</c> string.</param>
    /// <returns>A new access token along with its expiry time.</returns>
    /// <response code="200">Access token successfully refreshed.</response>
    /// <response code="400">Invalid refresh token payload or failed validation.</response>
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
    /// This endpoint registers a new user if the specified <c>email</c> is not already in use.
    /// After successful registration, an email confirmation link is generated and returned in the
    /// <c>Location</c> header of the response.
    /// <para> In production, this link would typically also be sent via email to the user,
    /// but for development and testing purposes, it can be retrieved directly from the response header. </para>
    /// </remarks>
    /// <param name="request">User registration details including <c>name</c>, <c>email</c>, and <c>password</c>.</param>
    /// <returns>
    /// A response confirming registration.
    /// The <c>Location</c> header contains the confirmation link the user must visit to activate their account.
    /// </returns>
    /// <response code="201">User successfully registered. Confirmation link included in <c>Location</c> header.</response>
    /// <response code="409">Registration failed because the email is already registered.</response>
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [HttpPost(ApiRoutes.Auth.Post.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        ServiceResult<RegisterResponse> result = await _authService.RegisterAsync(request);

        string? location = result.Data?.ConfirmationLink;
        result.Data = null; // strip data if you only want it in Location header

        return Created(location, result.MapToApiResponse());
    }

    /// <summary>
    /// Logs out the currently authenticated user by invalidating their session and token.
    /// </summary>
    /// <remarks>
    /// This endpoint requires the user to be authenticated (valid access token in the <c>Authorization</c> header).
    /// It revokes the user's current session/token, effectively ending their authenticated session.
    /// </remarks>
    /// <returns>A message confirming successful logout.</returns>
    /// <response code="200">Logout successful.</response>
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
