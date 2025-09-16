public interface IUserService
{
    Task<User> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto);

}