using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Auth;

namespace JobifyEcom.Services;

/// <summary>
/// Provides authentication-related services such as login, registration, and logout.
/// Service methods return <see cref="ServiceResult{T}"/> to communicate data, messages, and warnings.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Attempts to log in a user using the provided login request data.
    /// </summary>
    /// <param name="request">The login request containing user credentials.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The result contains a <see cref="TokenResponse"/> wrapped in a <see cref="ServiceResult{LoginResponse}"/>.
    /// </returns>
    Task<ServiceResult<TokenResponse>> LoginAsync(LoginRequest request);

    /// <summary>
    /// Refreshes the access token using a valid refresh token.
    /// </summary>
    /// <param name="request">The request containing the refresh token to validate and exchange for a new access token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="ServiceResult{TokenResponse}"/> which includes the new access token and its expiration time.
    /// </returns>
    Task<ServiceResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    /// Registers a new user using the provided registration data.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task<ServiceResult<object>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Logs out the current user, invalidating their authentication tokens as needed.
    /// </summary>
    /// <returns>A task representing the asynchronous logout operation.</returns>
    Task<ServiceResult<object>> LogoutAsync();
}
