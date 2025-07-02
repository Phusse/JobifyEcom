using Microsoft.AspNetCore.Mvc;
using JobifyEcom.DTOs;
using JobifyEcom.Common;
using Microsoft.AspNetCore.Authorization;

namespace JobifyEcom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
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
                    Token = "", // No token until email is confirmed
                    ExpiresAt = DateTime.MinValue
                }
            };
            return CreatedAtAction(nameof(Register), response); // 201 Created
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                return Conflict(new AuthResponse { Success = false, Message = ex.Message }); // 409

            return BadRequest(new AuthResponse { Success = false, Message = ex.Message }); // 400
        }
    }

    [HttpPost("login")]
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

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        try
        {
            await _authService.ConfirmEmailAsync(email, token);
            return Ok(new ApiResponse<string>(true, "Email confirmed successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(false, ex.Message));
        }
    }


    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        try
        {
            var email = User.FindFirst("email")?.Value;
            var role = User.FindFirst("role")?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new ApiResponse<object>(false, "User not authenticated"));

            return Ok(new ApiResponse<object>(true, "User info", new { Email = email, Role = role }));
        }
        catch
        {
            return StatusCode(500, new ApiResponse<string>(false, "Internal server error"));
        }
    }
}