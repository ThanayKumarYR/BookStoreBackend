using BusinessLayer.Interface;
using ModelLayer.UserModel;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserServiceBusinessLayer : IUserBusinessLayer
    {
        private readonly IUserRepositoryLayer _userRepository;

        public UserServiceBusinessLayer(IUserRepositoryLayer userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> CreateUserAsync(string fullName, string emailId, string password, string mobileNumber)
        {
            return await _userRepository.InsertUserAsync(fullName, emailId, password, mobileNumber);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task<string> GetUserByEmailIdAsync(UserLoginModel userLoginModel)
        {
            return await _userRepository.GetUserByEmailIdAsync(userLoginModel);
        }

        public async Task UpdateUserAsync(int userId, string fullName, string emailId, string password, string mobileNumber)
        {
            await _userRepository.UpdateUserAsync(userId, fullName, emailId, password, mobileNumber);
        }

        public async Task DeleteUserAsync(int userId)
        {
            await _userRepository.DeleteUserAsync(userId);
        }
    }
}
