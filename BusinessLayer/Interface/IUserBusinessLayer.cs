using RepositoryLayer.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IUserBusinessLayer
    {
        Task<int> CreateUserAsync(string fullName, string emailId, string password, string mobileNumber);
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(int userId, string fullName, string emailId, string password, string mobileNumber);
        Task DeleteUserAsync(int userId);
    }
}
