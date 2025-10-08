using INESPRE.Core.Models.Users;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var id = await _auth.RegisterAsync(request);
        return CreatedAtAction(nameof(Register), new { id }, new { userId = id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _auth.LoginAsync(request);
        if (!result.IsValid) return Unauthorized(new { message = "Usuario o contraseña inválidos" });
        return Ok(result);
    }

    [HttpPost("{userId:int}/change-password")]
    public async Task<IActionResult> ChangePassword(int userId, [FromBody] string newPassword)
    {
        await _auth.ChangePasswordAsync(userId, newPassword);
        return NoContent();
    }
}
