using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobifyEcom.Contracts;
using JobifyEcom.DTOs.Auth;
using JobifyEcom.DTOs;
using JobifyEcom.Services;
using System.Security.Claims;
using JobifyEcom.Extensions;

namespace JobifyEcom.Controllers;

[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost(ApiRoutes.Auth.Post.Login)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(ApiResponse<LoginResponse>.Ok(result.Data!, result.Message));
    }

    [HttpPost(ApiRoutes.Auth.Post.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Created(string.Empty, ApiResponse<object>.Ok(result.Data!, result.Message));
    }

    [Authorize]
    [HttpPatch(ApiRoutes.Auth.Patch.Logout)]
    public async Task<IActionResult> Logout()
    {
        User.TryGetUserId(out var id);
        var result = await _authService.LogoutAsync(id);
        return Ok(ApiResponse<object>.Ok(result.Data, result.Message));
    }
}
