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

namespace JobifyEcom.Services;

/// <summary>
/// Provides authentication-related operations such as login, logout, and registration.
/// </summary>
/// <param name="db">The database context.</param>
/// <param name="jwt">The JWT token service.</param>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
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
                ["Please enter both your email address and password to sign in."]
            );
        }

        User? user = await GetUserByEmailAsync(request.Email);

        if (user is null || !PasswordSecurity.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException(
                "Login failed.",
                ["We couldn't find an account with those credentials. Please check your email and password, then try again."]
            );
        }

        if (!user.IsEmailConfirmed)
        {
            throw new UnauthorizedException(
                "Email confirmation required.",
                ["Please confirm your email address before signing in. Check your inbox for the confirmation link."]
            );
        }

        if (user.IsLocked)
        {
            throw new UnauthorizedException(
                "Account locked.",
                ["Your account is currently locked. Contact support if you need help unlocking it."]
            );
        }

        user.SecurityStamp = Guid.NewGuid();
        await _db.SaveChangesAsync();

        TokenResponse response = GenerateTokenResponse(user);
        return ServiceResult<TokenResponse>.Create(response, "Signed in successfully.", GetTokenWarnings(response));
    }

    public async Task<ServiceResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new ValidationException(
                "No refresh token provided.",
                ["A refresh token is required to renew your session. Please provide a valid refresh token."]
            );
        }

        ClaimsPrincipal principal = ValidateToken(request.RefreshToken, TokenType.Refresh);
        ExtractTokenUserData(principal, out Guid userId, out Guid tokenSecurityStamp);

        User user = await _db.Users.AsNoTracking()
            .Include(u => u.WorkerProfile)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new UnauthorizedException(
                "Account not found.",
                ["We couldn't find an account for this session. It may have been deleted or changed."]
            );

        if (user.SecurityStamp != tokenSecurityStamp)
        {
            throw new UnauthorizedException(
                "Session invalid.",
                ["Your account security has changed. Please sign in again to continue."]
            );
        }

        if (user.IsLocked)
        {
            throw new UnauthorizedException(
                "Account locked.",
                ["Your account is currently locked. Contact support if you need help unlocking it."]
            );
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
            response,
            "Your session has been renewed. You are still signed in.",
            GetTokenWarnings(response)
        );
    }

    public async Task<ServiceResult<object>> LogoutAsync()
    {
        Guid? userId = _httpContextAccessor.HttpContext?.User.GetUserId()
            ?? throw new UnauthorizedException(
                "Authentication required.",
                ["You must be signed in to log out."]
            );

        User? user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId.Value)
            ?? throw new NotFoundException(
                "User not found.",
                ["We couldn't find your account. If this keeps happening, please contact support."]
            );

        user.SecurityStamp = Guid.Empty;
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(null, "You have been signed out.");
    }

    public async Task<ServiceResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        string normalizedEmail = NormalizeEmail(request.Email);

        if (await _db.Users.AnyAsync(u => u.Email == normalizedEmail))
        {
            throw new ConflictException(
                "Email already registered.",
                ["An account with this email address already exists. Please sign in or use a different email."]
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
            UpdatedAt = DateTime.UtcNow,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Build the link
        BuildConfirmationLink(user.Email, confirmationToken, out string confirmationLink, out List<string>? warnings);
        warnings ??= ["Email sending is not yet available. For now, the confirmation link is included in the Location header."];

        return ServiceResult<RegisterResponse>.Create(
            new RegisterResponse
            {
                ConfirmationLink = confirmationLink
            },
            "Registration successful! Please check your email for a confirmation link to activate your account.",
            warnings
        );
    }

    #region Private Helpers

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

        confirmationLink = $"{baseUrl}/{ApiRoutes.User.Patch.ConfirmEmail}?email={Uri.EscapeDataString(email)}&token={token}";
    }

    #endregion
}
