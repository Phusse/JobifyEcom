using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using JobifyEcom.Data;
using JobifyEcom.Helpers;
using JobifyEcom.DTOs;
using JobifyEcom.Models;
using JobifyEcom.DTOs.Auth;
using JobifyEcom.Exceptions;
using JobifyEcom.Enums;
using JobifyEcom.Security;
using JobifyEcom.Contracts;

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
internal class AuthService(AppDbContext db, JwtTokenGenerator jwt, IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly AppDbContext _db = db;
    private readonly JwtTokenGenerator _jwt = jwt;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException("Invalid credentials.", ["Email and password are required."]);
        }

        string normalizedEmail = request.Email.ToLowerInvariant().Trim();

        User? user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user is null || !PasswordSecurity.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid credentials.", ["The email or password you entered is incorrect."]);
        }

        if (!user.IsEmailConfirmed)
        {
            throw new UnauthorizedException("Email not confirmed.", ["Please confirm your email before logging in."]);
        }

        user.SecurityStamp = Guid.NewGuid();
        await _db.SaveChangesAsync();

        string token = _jwt.GenerateToken(user);
        DateTime expiresAt = JwtTokenReader.GetExpiryFromToken(token);

        LoginResponse response = new()
		{
            Token = token,
            ExpiresAt = expiresAt,
        };

        return ServiceResult<LoginResponse>.Create(response, "Login successful.");
    }

    public async Task<ServiceResult<object>> LogoutAsync(Guid? userId)
    {
        User user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException("Unable to find your user details. If this issue persists, please contact support.");

        user.SecurityStamp = Guid.Empty;
        await _db.SaveChangesAsync();

        return ServiceResult<object>.Create(null, "You have been logged out successfully.");
    }

    public async Task<ServiceResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        string normalizedEmail = request.Email.ToLowerInvariant().Trim();

        if (await _db.Users.AnyAsync(u => u.Email == normalizedEmail))
        {
            throw new ConflictException("User registration conflict.", ["A user with this email address already exists."]);
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
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        #region Rework Later

        // TODO: Find a better way to get the host if better
        // or revamp this to scale better
        HttpRequest? requestHttp = _httpContextAccessor.HttpContext?.Request;
        string baseUrl;
        List<string>? warnings = null;

        if (requestHttp is null || string.IsNullOrEmpty(requestHttp.Host.Value))
        {
            baseUrl = "http://localhost:5122";
            warnings = ["Base URL could not be determined from the HTTP request. Using fallback URL."];
        }
        else
        {
            baseUrl = $"{requestHttp.Scheme}://{requestHttp.Host.Value}";
        }

        // TODO: Replace with the actual confirmation route when implemented
        string confirmationLink = $"{baseUrl}/{ApiRoutes.Auth.Patch.Logout}?email={user.Email}&token={confirmationToken}";

        RegisterResponse response = new()
        {
            ConfirmationLink = confirmationLink,
        };

        #endregion

        return ServiceResult<RegisterResponse>.Create(response, "Registration successful. Please check your email to confirm your account.", warnings);
    }
}
