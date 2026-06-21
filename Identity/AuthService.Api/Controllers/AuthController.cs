using AuthService.Application.DTOs;
using AuthService.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        [FromServices] RegisterUserCommand command)
    {
        await command.ExecuteAsync(request);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginUserRequest request,
        [FromServices] LoginUserCommand command)
    {
        var result = await command.ExecuteAsync(request);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetUserDetails(
        [FromServices] GetUserDetailsQuery query)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await query.ExecuteAsync(userId);
        return Ok(result);
    }
}