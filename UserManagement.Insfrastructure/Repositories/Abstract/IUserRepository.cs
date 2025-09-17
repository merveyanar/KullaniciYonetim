namespace UserManagement.Domain
{
    public interface IUserRepository
    {
        User GetUserById(int id);
        void UpdateUser(User user);
        // İstersen: void AddUser(User user); vs.
        List<User> GetUsers();
        Task<User?> GetByIdAsync(int id);
        Task DeleteAsync(User user);
        Task UpdateAsync(User user);

    }
}
