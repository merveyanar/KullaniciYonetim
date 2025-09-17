// Controllers/UsersController.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var userId = GetUserIdFromToken();
        if (userId == null)
            return Unauthorized();

        var users = _userService.GetUsers(userId.Value);
        if (users == null)
            return NotFound();

        return Ok(users);
    }
    // GET: api/users/profile
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var userId = GetUserIdFromToken();
        if (userId == null)
            return Unauthorized();

        var profile = _userService.GetProfile(userId.Value);
        if (profile == null)
            return NotFound();

        return Ok(profile);
    }

    // PUT: api/users/profile
    [HttpPut("profile")]
    public IActionResult UpdateProfile([FromBody] UserProfileDto dto)
    {
        var userId = GetUserIdFromToken();
        if (userId == null)
            return Unauthorized();

        _userService.UpdateProfile(userId.Value, dto);
        return Ok(new { message = "Profil başarıyla güncellendi." });
    }

    // Kullanıcı ID'sini JWT Token'dan al
    private int? GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }
    // GET api/users/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Role
        });
    }
    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (!success)
            return NotFound(new { message = "Kullanıcı bulunamadı." });

        return Ok(new { message = "Kullanıcı başarıyla silindi." });
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        var success = await _userService.UpdateUserAsync(id, dto);
        if (!success)
            return NotFound(new { message = "Kullanıcı bulunamadı." });

        return Ok(new { message = "Kullanıcı başarıyla güncellendi." });
    }
}



