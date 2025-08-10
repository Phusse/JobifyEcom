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
    /// The result contains a <see cref="LoginResponse"/> wrapped in a <see cref="ServiceResult{T}"/>.
    /// </returns>
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);

    /// <summary>
    /// Registers a new user using the provided registration data.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The result contains a <see cref="ServiceResult{T}"/> wrapping an <see cref="object"/>,
    /// which can hold any additional data or be null when no data is returned.
    /// </returns>
    Task<ServiceResult<object>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Logs out the current user, invalidating their authentication tokens as needed.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to log out.</param>
    /// <returns>A task representing the asynchronous logout operation.</returns>
    Task<ServiceResult<object>> LogoutAsync(Guid userId);
}
