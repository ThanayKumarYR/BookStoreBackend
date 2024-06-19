using ModelLayer.UserModel;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRepositoryLayer
    {
        Task<int> InsertUserAsync(string fullName, string emailId, string password, string mobileNumber);
        Task<List<User>> GetUsersAsync();
        Task<string> GetUserByEmailIdAsync(UserLoginModel userLoginModel);
        Task UpdateUserAsync(int userId, string fullName, string emailId, string password, string mobileNumber);
        Task DeleteUserAsync(int userId);
    }
}
