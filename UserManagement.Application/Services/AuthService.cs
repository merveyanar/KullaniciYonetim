
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _configuration = configuration;
        _context = context;

    }
    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == dto.Username);
        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
            throw new Exception("Kullanıcı adı veya şifre hatalı");

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computed.SequenceEqual(passwordHash);
    }

    public async Task<User> RegisterAsync(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            throw new Exception("Kullanıcı zaten mevcut");

        CreatePasswordHash(dto.Password, out byte[] hash, out byte[] salt);

        var user = new User
        {
            Username = dto.Username,
            FirstName="",
            LastName="",
            Phone="",
            Email="",
            ProfileImageUrl ="",
            PasswordHash = hash,
            PasswordSalt = salt,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;

    }

    private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
    var user = await _context.Users.FindAsync(userId);
    if (user == null)
        throw new Exception("User not found");

    if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
        throw new InvalidCredentialException("Current password is incorrect");

    CreatePasswordHash(dto.NewPassword, out byte[] newHash, out byte[] newSalt);

    user.PasswordHash = newHash;
    user.PasswordSalt = newSalt;

    _context.Users.Update(user);
    await _context.SaveChangesAsync();
    }
}