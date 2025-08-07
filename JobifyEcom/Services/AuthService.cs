using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using JobifyEcom.Data;
using JobifyEcom.Helpers;
using JobifyEcom.DTOs;
using JobifyEcom.Models;

namespace JobifyEcom.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtHelper _jwt;

    public AuthService(AppDbContext db, JwtHelper jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            throw new Exception("User already exists");

        var confirmationToken = Guid.NewGuid().ToString();

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Role = dto.Role,
            IsEmailConfirmed = false,
            EmailConfirmationToken = confirmationToken
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Return link to simulate sending via email
        return $"https://localhost:5001/api/auth/confirm?email={user.Email}&token={confirmationToken}";
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        if (!user.IsEmailConfirmed)
            throw new Exception("Please confirm your email before logging in.");

        return _jwt.GenerateToken(user.Id, user.Email, user.Role.ToString());
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            throw new Exception("User not found");
        return user;
    }

    public async Task<User> ConfirmEmailAsync(string email, string token)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            throw new Exception("User not found");

        if (user.EmailConfirmationToken != token)
            throw new Exception("Invalid token. User ID not found.");

        user.IsEmailConfirmed = true;
        user.EmailConfirmationToken = null;

        await _db.SaveChangesAsync();

        return user;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string inputPassword, string storedHash)
    {
        return HashPassword(inputPassword) == storedHash;
    }
}
