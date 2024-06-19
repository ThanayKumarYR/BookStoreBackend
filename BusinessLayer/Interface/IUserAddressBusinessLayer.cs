using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IUserAddressBusinessLayer
    {
        Task<int> InsertAddressAsync(UserAddress userAddress);
        Task<List<UserAddress>> GetAddressesAsync();
        Task<UserAddress> GetAddressByIdAsync(int addressId);
        Task UpdateAddressAsync(UserAddress userAddress);
        Task DeleteAddressAsync(int addressId);
    }
}
