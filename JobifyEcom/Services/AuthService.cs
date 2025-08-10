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

namespace JobifyEcom.Services;

internal class AuthService(AppDbContext db, JwtTokenGenerator jwt) : IAuthService
{
    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest dto)
    {
        if (dto.Email is null or { Length: 0 } || dto.Password is null or { Length: 0 })
            throw new UnauthorizedException("Invalid credentials.", ["Email and password are required."]);

        string email = dto.Email.ToLowerInvariant().Trim();
        string password = dto.Password;

        string normalizedEmail = dto.Email.ToLowerInvariant().Trim();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.", ["Email or password is incorrect."]);

        if (!user.IsEmailConfirmed)
            throw new UnauthorizedException("Email not confirmed.", ["Please confirm your email before logging in."]);

        user.SecurityStamp = Guid.NewGuid();
        await db.SaveChangesAsync();

        string token = jwt.GenerateToken(user);
        DateTime expiresAt = JwtTokenReader.GetExpiryFromToken(token);

        var response = new LoginResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Token = token,
            ExpiresAt = expiresAt,
            Role = user.Role,
        };

        return ServiceResult<LoginResponse>.Create(response, "Login successful");
    }

    public async Task<ServiceResult<object>> LogoutAsync(Guid userId)
    {
        User user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException("User not found");

        user.SecurityStamp = Guid.Empty;
        await db.SaveChangesAsync();

        return ServiceResult<object>.Create(null, "Logged out successfully");
    }

    public async Task<ServiceResult<object>> RegisterAsync(RegisterRequest dto)
    {
        if (await db.Users.AnyAsync(u => u.Email == dto.Email.ToLowerInvariant().Trim()))
            throw new AppException(400, "User already exists", ["A user with this email already exists."]);

        var confirmationToken = Guid.NewGuid();

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email.ToLowerInvariant().Trim(),
            PasswordHash = HashPassword(dto.Password),
            IsEmailConfirmed = false,
            EmailConfirmationToken = confirmationToken,
            Role = UserRole.Customer,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        string confirmationLink = $"https://localhost:5001/api/auth/confirm?email={user.Email}&token={confirmationToken}";

        return ServiceResult<object>.Create(new { ConfirmationLink = confirmationLink }, "Registration successful. Please confirm your email.");
    }

    private static string HashPassword(string password)
    {
        byte[] hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string inputPassword, string storedHash)
    {
        return HashPassword(inputPassword) == storedHash;
    }
}
