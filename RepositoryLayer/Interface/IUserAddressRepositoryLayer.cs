using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserAddressRepositoryLayer
    {
        Task<int> InsertAddressAsync(UserAddress userAddress);
        Task<List<UserAddress>> GetAddressesAsync();
        Task<UserAddress> GetAddressByIdAsync(int addressId);
        Task UpdateAddressAsync(UserAddress userAddress);
        Task DeleteAddressAsync(int addressId);
    }
}
