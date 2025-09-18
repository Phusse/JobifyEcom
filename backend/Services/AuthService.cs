using Microsoft.EntityFrameworkCore;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.Models;
using JobifyEcom.DTOs.Auth;
using JobifyEcom.Exceptions;
using JobifyEcom.Security;
using JobifyEcom.Contracts.Routes;
using System.Security.Claims;
using JobifyEcom.Extensions;
using JobifyEcom.Contracts.Errors;
using JobifyEcom.Contracts.Results;
using JobifyEcom.Helpers;

namespace JobifyEcom.Services;

/// <summary>
/// Provides authentication-related operations such as login, logout, and registration.
/// </summary>
/// <param name="db">The database context.</param>
/// <param name="jwt">The JWT token service.</param>
/// <param name="appContextService">The HTTP context accessor.</param>
internal class AuthService(AppDbContext db, JwtTokenService jwt, AppContextService appContextService) : IAuthService
{
    private readonly AppDbContext _db = db;
    private readonly JwtTokenService _jwt = jwt;
    private readonly AppContextService _appContextService = appContextService;

    private readonly TimeSpan _accessTokenLifetime = TimeSpan.FromMinutes(30);
    private readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(7);

    public async Task<ServiceResult<TokenResponse>> LoginAsync(LoginRequest request)
    {
        User? user = await GetUserByEmailAsync(request.Email);

        if (user is null || !PasswordSecurity.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new AppException(ErrorCatalog.InvalidCredentials);
        }

        if (!user.IsEmailConfirmed)
        {
            throw new AppException(ErrorCatalog.EmailNotConfirmed);
        }

        if (user.IsLocked)
        {
            throw new AppException(ErrorCatalog.AccountLocked);
        }

        user.SecurityStamp = Guid.NewGuid();
        await _db.SaveChangesAsync();

        TokenResponse response = GenerateTokenResponse(user);

        return ServiceResult<TokenResponse>.Create(
            ResultCatalog.LoginSuccessful.AppendDetails(
                GetTokenWarnings(response)?.ToArray() ?? []
            ),
            response
        );
    }

    public async Task<ServiceResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        ClaimsPrincipal principal = ValidateToken(request.RefreshToken, TokenType.Refresh);
        ExtractTokenUserData(principal, out Guid userId, out Guid tokenSecurityStamp);

        User user = await _appContextService.GetCurrentUserAsync();

        if (user.SecurityStamp != tokenSecurityStamp)
        {
            throw new AppException(ErrorCatalog.SessionExpired);
        }

        if (user.IsLocked)
        {
            throw new AppException(ErrorCatalog.AccountLocked);
        }

        string newAccessToken = _jwt.GenerateToken(user, _accessTokenLifetime, TokenType.Access);

        TokenResponse response = new()
        {
            AccessToken = newAccessToken,
            AccessTokenExpiresAt = JwtTokenReader.GetExpiryFromToken(newAccessToken),
            RefreshToken = request.RefreshToken,
            RefreshTokenExpiresAt = JwtTokenReader.GetExpiryFromToken(request.RefreshToken),
        };

        return ServiceResult<TokenResponse>.Create(
            ResultCatalog.RefreshSuccessful.AppendDetails(
                GetTokenWarnings(response)?.ToArray() ?? []
            ),
            response
        );
    }

    public async Task<ServiceResult<object>> LogoutAsync()
    {
        User user = await _appContextService.GetCurrentUserAsync();

        user.SecurityStamp = Guid.Empty;
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(ResultCatalog.LogoutSuccessful);
    }

    public async Task<ServiceResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        string normalizedEmail = NormalizeEmail(request.Email);

        if (await _db.Users.AnyAsync(u => u.Email == normalizedEmail))
        {
            throw new AppException(ErrorCatalog.AlreadyRegistered);
        }

        Guid confirmationToken = Guid.NewGuid();

        User user = new()
        {
            Name = request.Name,
            Email = normalizedEmail,
            PasswordHash = PasswordSecurity.HashPassword(request.Password),
            IsEmailConfirmed = false,
            EmailConfirmationToken = confirmationToken,
            UpdatedAt = DateTime.UtcNow,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Build the link
        HttpRequest? httpRequest = _appContextService.HttpRequest;
        string confirmationLink = httpRequest is not null
            ? ResourceUrlBuilder.BuildConfirmationLink(httpRequest, user.Email, confirmationToken)
            : string.Empty;

        return ServiceResult<RegisterResponse>.Create(
            ResultCatalog.RegistrationSuccessful.AppendDetails(
                "Email sending is not yet avaible. For now, the confirmation link is included in the the Location header."
            ),
            new RegisterResponse
            {
                ConfirmationLink = confirmationLink,
            }
        );
    }

    private static string NormalizeEmail(string email) => email.ToLowerInvariant().Trim();

    private async Task<User?> GetUserByEmailAsync(string email)
    {
        // Eagerly include the WorkerProfile to ensure role and claims resolution works correctly
        // when generating the JWT.
        return await _db.Users
            .Include(u => u.WorkerProfile)
            .FirstOrDefaultAsync(u => u.Email == NormalizeEmail(email));
    }

    private TokenResponse GenerateTokenResponse(User user)
    {
        string accessToken = _jwt.GenerateToken(user, _accessTokenLifetime, TokenType.Access);
        string refreshToken = _jwt.GenerateToken(user, _refreshTokenLifetime, TokenType.Refresh);

        return new TokenResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = JwtTokenReader.GetExpiryFromToken(accessToken),
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = JwtTokenReader.GetExpiryFromToken(refreshToken),
        };
    }

    private static List<string>? GetTokenWarnings(TokenResponse response)
    {
        List<string>? warnings = null;

        if (response.AccessTokenExpiresAt is null)
        {
            warnings ??= [];
            warnings.Add("Unable to determine when your access session expires.");
        }

        if (response.RefreshTokenExpiresAt is null)
        {
            warnings ??= [];
            warnings.Add("Unable to determine when your refresh session expires.");
        }

        return warnings;
    }

    private ClaimsPrincipal ValidateToken(string token, TokenType tokenType)
    {
        ClaimsPrincipal? principal = _jwt.ValidateToken(token, tokenType);

        if (principal is not null) return principal;

        // Try validating without token type restriction to detect token type errors
        ClaimsPrincipal? genericPrincipal = _jwt.ValidateToken(token, null);

        if (genericPrincipal is not null)
        {
            string? tokenTypeClaim = genericPrincipal.GetTokenType();

            if (!string.Equals(tokenTypeClaim, tokenType.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                throw new AppException(ErrorCatalog.TokenTypeInvalid.AppendDetails(
                    [$"The token provided is not a {tokenType} token. Please provide a valid {tokenType} token."]
                ));
            }
        }

        throw new AppException(ErrorCatalog.SessionExpired);
    }

    private static void ExtractTokenUserData(ClaimsPrincipal principal, out Guid userId, out Guid securityStamp)
    {
        string? userIdClaim = principal.GetUserId().ToString();
        string? securityStampClaim = principal.GetSecurityStamp().ToString();

        if (!Guid.TryParse(userIdClaim, out userId) || !Guid.TryParse(securityStampClaim, out securityStamp))
        {
            throw new AppException(ErrorCatalog.SessionInvalid);
        }
    }
}
