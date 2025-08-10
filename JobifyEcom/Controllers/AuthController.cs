using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobifyEcom.Contracts;
using JobifyEcom.DTOs.Auth;
using JobifyEcom.DTOs;
using JobifyEcom.Services;
using JobifyEcom.Extensions;

namespace JobifyEcom.Controllers;

/// <summary>
/// Handles user authentication operations including login, registration, and logout.
/// </summary>
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Authenticates a user and issues a JSON Web Token (JWT) upon successful login.
    /// </summary>
    /// <remarks>
    /// This endpoint verifies the user's *email* and *password* credentials.
    /// Upon successful authentication, it returns a **JWT token**, which must be included
    /// in the <c>Authorization</c> header of subsequent requests to access protected resources.
    /// </remarks>
    /// <param name="request">The login credentials including *email* and *password*.</param>
    /// <returns>A **JWT token** and a success message if authentication succeeds.</returns>
    /// <response code="200">Login successful. **JWT token** returned.</response>
    /// <response code="400">Login request contains invalid data or fails validation.</response>
    /// <response code="401">Authentication failed due to incorrect credentials.</response>
    [HttpPost(ApiRoutes.Auth.Post.Login)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        ServiceResult<LoginResponse> result = await _authService.LoginAsync(request);
        return Ok(ApiResponse<LoginResponse>.Ok(result.Data, result.Message));
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
    [HttpPost(ApiRoutes.Auth.Post.Register)]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        ServiceResult<RegisterResponse> result = await _authService.RegisterAsync(request);
        return Created(string.Empty, ApiResponse<RegisterResponse>.Ok(result.Data, result.Message));
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
    [Authorize]
    [HttpPatch(ApiRoutes.Auth.Patch.Logout)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Logout()
    {
        User.TryGetUserId(out var id);
        ServiceResult<object> result = await _authService.LogoutAsync(id);
        return Ok(ApiResponse<object>.Ok(result.Data, result.Message));
    }
}
