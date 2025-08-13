using JobifyEcom.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
	private readonly IUserService _userService = userService;

	[HttpPost]
	public async Task<IActionResult> Call()
	{
		return Ok("It worked hahaha.");
	}
}
