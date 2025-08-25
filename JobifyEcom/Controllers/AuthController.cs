using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobifyEcom.Contracts;
using JobifyEcom.DTOs.Auth;
using JobifyEcom.DTOs;
using JobifyEcom.Services;

namespace JobifyEcom.Controllers;

/// <summary>
/// Handles user authentication operations including login, registration, and logout.
/// </summary>
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Authenticates a user and issues JSON Web Tokens (JWT) upon successful login.
    /// </summary>
    /// <remarks>
    /// This endpoint verifies the user's *email* and *password* credentials.
    /// Upon successful authentication, it returns both:
    /// <list type="bullet">
    ///   <item><description>A short-lived access JWT token, which must be included
    ///   in the <c>Authorization</c> header of subsequent requests to access protected resources.</description></item>
    ///   <item><description>A long-lived refresh JWT token, which is used to obtain new access tokens
    ///   without requiring the user to re-enter credentials.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="request">The login credentials including *email* and *password*.</param>
    /// <returns>A response containing both access and refresh JWT tokens along with their expiration times.</returns>
    /// <response code="200">Login successful. Access and refresh JWT tokens returned.</response>
    /// <response code="400">Login request contains invalid data or fails validation.</response>
    /// <response code="401">Authentication failed due to incorrect credentials.</response>
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [HttpPost(ApiRoutes.Auth.Post.Login)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        ServiceResult<TokenResponse> result = await _authService.LoginAsync(request);
        return Ok(ApiResponse<TokenResponse>.Ok(result.Data, result.Message, result.Errors));
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token.
    /// </summary>
    /// <remarks>
    /// This endpoint allows the client to obtain a new short-lived access token without
    /// re-entering credentials, provided that the supplied refresh token is still valid.
    /// </remarks>
    /// <param name="request">The refresh token request containing the refresh token string.</param>
    /// <returns>
    /// A new access token along with its expiry, while the provided refresh token remains unchanged.
    /// Throws an error if the request is invalid or unauthorized.
    /// </returns>
    /// <response code="200">Token successfully refreshed.</response>
    /// <response code="400">Invalid request payload or failed validation.</response>
    /// <response code="401">Refresh token is invalid, expired, or revoked.</response>
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [HttpPost(ApiRoutes.Auth.Post.Refresh)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        ServiceResult<TokenResponse> result = await _authService.RefreshTokenAsync(request);
        return Ok(ApiResponse<TokenResponse>.Ok(result.Data, result.Message, result.Errors));
    }

    /// <summary>
    /// Registers a new user account with the provided information.
    /// </summary>
    /// <remarks>
    /// Creates a new user profile if the *email address* is not already in use.
    /// Registration requires valid user details such as *name*, *email*, and *password*.
    /// </remarks>
    /// <param name="request">User registration details.</param>
    /// <returns>Confirmation of successful registration or an error indicating conflict.</returns>
    /// <response code="201">User successfully registered.</response>
    /// <response code="409">Registration failed because the email is already registered.</response>
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [HttpPost(ApiRoutes.Auth.Post.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        ServiceResult<object> result = await _authService.RegisterAsync(request);
        return Created(string.Empty, ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
    }

    /// <summary>
    /// Logs out the currently authenticated user by invalidating their session or token.
    /// </summary>
    /// <remarks>
    /// This endpoint requires the user to be *authenticated*.
    /// It revokes the user's authentication token, effectively ending their session.
    /// </remarks>
    /// <returns>A confirmation message upon successful logout or an error if the user was not found.</returns>
    /// <response code="200">Logout completed successfully.</response>
    /// <response code="404">User not found or already logged out.</response>
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [Authorize]
    [HttpPatch(ApiRoutes.Auth.Patch.Logout)]
    public async Task<IActionResult> Logout()
    {
        ServiceResult<object> result = await _authService.LogoutAsync();
        return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
    }
}
