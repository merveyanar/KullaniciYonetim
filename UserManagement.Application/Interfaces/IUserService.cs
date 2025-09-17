public interface IUserService
{
    UserProfileDto GetProfile(int userId);
    void UpdateProfile(int userId, UserProfileDto dto);
    List<UserDto> GetUsers(int userId);
    Task<User?> GetByIdAsync(int id);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> UpdateUserAsync(int id, UpdateUserDto dto);

}