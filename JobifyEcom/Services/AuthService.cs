using Microsoft.EntityFrameworkCore;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.Models;
using JobifyEcom.DTOs.Auth;
using JobifyEcom.Exceptions;
using JobifyEcom.Enums;
using JobifyEcom.Security;
using JobifyEcom.Contracts;
using System.Security.Claims;
using JobifyEcom.Extensions;

namespace JobifyEcom.Services;

/// <summary>
/// Provides authentication-related operations such as login, logout, and registration.
/// </summary>
/// <remarks>
/// This service handles:
/// <list type="bullet">
///   <item><description>User authentication via email and password.</description></item>
///   <item><description>JWT generation for authenticated users.</description></item>
///   <item><description>Security stamp updates to support token invalidation on logout or credential changes.</description></item>
///   <item><description>New user registration with email confirmation tokens.</description></item>
/// </list>
/// </remarks>
internal class AuthService(AppDbContext db, JwtTokenService jwt, IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly AppDbContext _db = db;
    private readonly JwtTokenService _jwt = jwt;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private readonly TimeSpan _accessTokenLifetime = TimeSpan.FromMinutes(30);
    private readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(7);

    public async Task<ServiceResult<TokenResponse>> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException(
                "Missing login details.",
                ["Please enter both your email address and password to continue."]
            );
        }

        User? user = await GetUserByEmailAsync(request.Email);

        if (user is null || !PasswordSecurity.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException(
                "Login failed.",
                ["We couldn't find an account with those details. Double-check your email and password, then try again."]
            );
        }

        if (!user.IsEmailConfirmed)
        {
            throw new UnauthorizedException(
                "Email confirmation required.",
                ["You need to confirm your email before logging in. Check your inbox for the confirmation link."]
            );
        }

        user.SecurityStamp = Guid.NewGuid();
        await _db.SaveChangesAsync();

        TokenResponse response = GenerateTokenResponse(user);
        return ServiceResult<TokenResponse>.Create(response, "Login successful.", GetTokenWarnings(response));
    }

    public async Task<ServiceResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new ValidationException(
                "No refresh token provided.",
                ["We couldn't continue your session because a refresh token was missing from the request."]
            );
        }

        ClaimsPrincipal principal = ValidateToken(request.RefreshToken, TokenType.Refresh);
        ExtractTokenUserData(principal, out Guid userId, out Guid tokenSecurityStamp);

        User user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new UnauthorizedException(
                "Account not found.",
                ["We couldn't find an account linked to this session. It may have been deleted or changed."]
            );

        if (user.SecurityStamp != tokenSecurityStamp)
        {
            throw new UnauthorizedException(
                "Session no longer valid.",
                ["Your account security has changed. Please sign in again to continue."]
            );
        }

        TokenResponse response = GenerateTokenResponse(user);
        return ServiceResult<TokenResponse>.Create(
            response,
            "Your session has been renewed successfully.",
            GetTokenWarnings(response)
        );
    }

    public async Task<ServiceResult<object>> LogoutAsync()
    {
        Guid? userId = (_httpContextAccessor.HttpContext?.User.GetUserId())
            ?? throw new UnauthorizedException(
                "Authentication required.",
                ["You must be logged in to log out."]
            );

        User? user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId.Value)
            ?? throw new NotFoundException(
                "User not found.",
                ["We couldn't find your account information. If this keeps happening, please contact support."]
            );

        // Invalidate tokens by clearing the security stamp
        user.SecurityStamp = Guid.Empty;
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(null, "You've been signed out successfully.");
    }

    public async Task<ServiceResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        string normalizedEmail = NormalizeEmail(request.Email);

        if (await _db.Users.AnyAsync(u => u.Email == normalizedEmail))
        {
            throw new ConflictException(
                "Email already registered.",
                ["An account with this email address already exists. Try signing in instead."]
            );
        }

        Guid confirmationToken = Guid.NewGuid();

        User user = new()
        {
            Name = request.Name,
            Email = normalizedEmail,
            PasswordHash = PasswordSecurity.HashPassword(request.Password),
            IsEmailConfirmed = false,
            EmailConfirmationToken = confirmationToken,
            Role = UserRole.Customer,
            UpdatedAt = DateTime.UtcNow,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        BuildConfirmationLink(user.Email, confirmationToken, out string confirmationLink, out List<string>? warnings);

        RegisterResponse response = new()
        {
            ConfirmationLink = confirmationLink
        };

        return ServiceResult<RegisterResponse>.Create(
            response,
            "Registration complete. Please check your inbox for a confirmation link to activate your account.",
            warnings
        );
    }

    #region Private Helpers

    private static string NormalizeEmail(string email) => email.ToLowerInvariant().Trim();

    private async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == NormalizeEmail(email));
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
                throw new UnauthorizedException(
                    "Invalid token type.",
                    [$"The token provided is not a {tokenType} token. Please provide a valid {tokenType} token."]
                );
            }
        }

        throw new UnauthorizedException(
            "Session expired.",
            ["Your session has expired or is no longer valid. Please sign in again."]
        );
    }

    private static void ExtractTokenUserData(ClaimsPrincipal principal, out Guid userId, out Guid securityStamp)
    {
        string? userIdClaim = principal.GetUserId().ToString();
        string? securityStampClaim = principal.GetSecurityStamp().ToString();

        if (!Guid.TryParse(userIdClaim, out userId) || !Guid.TryParse(securityStampClaim, out securityStamp))
        {
            throw new UnauthorizedException(
                "Session data invalid.",
                ["We couldn't verify your session details. Please login again."]
            );
        }
    }

    private void BuildConfirmationLink(string email, Guid token, out string confirmationLink, out List<string>? warnings)
    {
        HttpRequest? requestHttp = _httpContextAccessor.HttpContext?.Request;
        string baseUrl;
        warnings = null;

        if (requestHttp is null || string.IsNullOrEmpty(requestHttp.Host.Value))
        {
            baseUrl = "http://localhost:5122";
            warnings =
            [
                "Unable to determine the server address from the request. Using a default fallback URL."
            ];
        }
        else
        {
            baseUrl = $"{requestHttp.Scheme}://{requestHttp.Host.Value}";
        }

        // TODO: Update this route to the actual email confirmation endpoint when available
        confirmationLink = $"{baseUrl}/{ApiRoutes.Auth.Patch.Logout}?email={email}&token={token}";
    }

    #endregion
}
