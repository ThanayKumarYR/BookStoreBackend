

using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IAuthServiceRepositoryLayer
    {
        public string GenerateJwtToken(User user);
        public string GenerateJwtTokenForgetPassword(User user);
    }
}