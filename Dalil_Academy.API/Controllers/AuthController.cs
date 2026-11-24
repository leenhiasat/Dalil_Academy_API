using Dalil_Academy.Core.DTOs.Auth;
using Dalil_Academy.Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace Dalil_Academy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto dto)
    {
        var result = await authService.RegisterAsync(dto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
    {
        var result = await authService.LoginAsync(dto);
        return Ok(result);
    }
}