using Microsoft.AspNetCore.Mvc;
using JobifyEcom.DTOs;
using Microsoft.AspNetCore.Authorization;
using JobifyEcom.Contracts;
using JobifyEcom.Services;

namespace JobifyEcom.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost(ApiRoutes.Auth.Post.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var confirmLink = await _authService.RegisterAsync(dto);
            var user = await _authService.GetUserByEmailAsync(dto.Email);

            var response = new AuthResponse
            {
                Success = true,
                Message = $"Registration successful. Please confirm your email via: {confirmLink}",
                Data = new AuthData
                {
                    Id = user.Id,
                    Name = user.Email.Split('@')[0],
                    Email = user.Email,
                    Token = "",
                    ExpiresAt = DateTime.MinValue
                }
            };
            return CreatedAtAction(nameof(Register), response);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                return Conflict(new AuthResponse { Success = false, Message = ex.Message }); // 409

            return BadRequest(new AuthResponse { Success = false, Message = ex.Message }); // 400
        }
    }

    [HttpPost(ApiRoutes.Auth.Post.Login)]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var user = await _authService.GetUserByEmailAsync(dto.Email);

            if (!user.IsEmailConfirmed)
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Please confirm your email before logging in."
                });

            var token = await _authService.LoginAsync(dto);

            var response = new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Data = new AuthData
                {
                    Id = user.Id,
                    Name = user.Email.Split('@')[0],
                    Email = user.Email,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(3)
                }
            };
            return Ok(response); // 200 OK
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Invalid credentials", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new AuthResponse { Success = false, Message = ex.Message }); // 401

            return BadRequest(new AuthResponse { Success = false, Message = ex.Message }); // 400 fallback
        }
    }

    [HttpGet(ApiRoutes.Auth.Post.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        try
        {
            var user = await _authService.ConfirmEmailAsync(email, token);

            var response = new AuthResponse
            {
                Success = true,
                Message = "Email confirmed successfully",
                Data = new AuthData
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Token = "", // You can skip token here or issue a fresh one if you prefer
                    ExpiresAt = DateTime.UtcNow
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [Authorize]
    [HttpGet(ApiRoutes.Auth.Get.Me)]
    public IActionResult Me()
    {
        try
        {
            var email = User.FindFirst("email")?.Value;
            var role = User.FindFirst("role")?.Value;

            Console.WriteLine($"[Debug] Email: {email}, Role: {role}, User: {User}");

            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(ApiResponse<object?>.Fail(null, "User not authenticated."));

            return Ok(ApiResponse<object>.Ok(new { Email = email, Role = role }));
        }
        catch
        {
            return StatusCode(500, ApiResponse<object?>.Fail("Internal server error."));
        }
    }
}