using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = await _userService.RegisterAsync(dto);
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        // var token = await _userService.LoginAsync(dto);
        // return Ok(new { token });
        try
        {
            var token = await _userService.LoginAsync(dto);
            return Ok(new { token });
        }
        catch (InvalidCredentialException)
        {
            return Unauthorized(new { error = "Invalid username or password" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
[Authorize]
[HttpPost("change-password")]
public async Task<IActionResult> ChangePassword( ChangePasswordDto dto)
{
    try
    {
        // JWT içinden kullanıcı ID'sini al
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _userService.ChangePasswordAsync(userId, dto);
        return Ok(new { message = "Password changed successfully" });
    }
    catch (InvalidCredentialException ex)
    {
        return Unauthorized(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = ex.Message });
    }
}
}
