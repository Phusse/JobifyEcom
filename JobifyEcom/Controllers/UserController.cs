using JobifyEcom.Contracts;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.User;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

[Authorize]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
	private readonly IUserService _userService = userService;

	[HttpPost(ApiRoutes.Users.Get.Me)]
	public async Task<IActionResult> Call()
	{
		ServiceResult<ProfileResponse> result = await _userService.GetCurrentUserAsync();
		return Ok(ApiResponse<ProfileResponse>.Ok(result.Data, result.Message, result.Errors));
	}
}
