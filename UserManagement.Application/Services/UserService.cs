
using Microsoft.Extensions.Configuration;
using UserManagement.Domain;
using UserManagement.Domain.Concrete;


public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public UserService(AppDbContext context, IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _context = context;
        _userRepository = userRepository;
    }
    public List<UserDto> GetUsers(int userId)
    {
        var user = _userRepository.GetUserById(userId);
        if (user == null)
            return null;

        var users = _userRepository.GetUsers().Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Role = u.Role
        }).ToList();

        return users;

    }
    public UserProfileDto GetProfile(int userId)
    {
        var user = _userRepository.GetUserById(userId);
        if (user == null)
            return null;

        return new UserProfileDto
        {
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            ProfileImageUrl = user.ProfileImageUrl
        };
    }

    public void UpdateProfile(int userId, UserProfileDto dto)
    {
        var user = _userRepository.GetUserById(userId);
        if (user == null)
            return;

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.Phone = dto.Phone;
        user.ProfileImageUrl = dto.ProfileImageUrl;

        _userRepository.UpdateUser(user);
    }
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        await _userRepository.DeleteAsync(user);
        return true;
    }
public async Task<bool> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.Role = dto.Role;

        await _userRepository.UpdateAsync(user);
        return true;
    }
}