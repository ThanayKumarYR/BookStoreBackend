using BusinessLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserAddressServiceBusinessLayer : IUserAddressBusinessLayer
    {
        private readonly IUserAddressRepositoryLayer _userAddressRepositoryLayer;

        public UserAddressServiceBusinessLayer(IUserAddressRepositoryLayer userAddressRepositoryLayer)
        {
            _userAddressRepositoryLayer = userAddressRepositoryLayer;
        }

        public async Task<int> InsertAddressAsync(UserAddress userAddress)
        {
            return await _userAddressRepositoryLayer.InsertAddressAsync(userAddress);
        }

        public async Task<List<UserAddress>> GetAddressesAsync()
        {
            return await _userAddressRepositoryLayer.GetAddressesAsync();
        }

        public async Task<UserAddress> GetAddressByIdAsync(int addressId)
        {
            return await _userAddressRepositoryLayer.GetAddressByIdAsync(addressId);
        }

        public async Task UpdateAddressAsync(UserAddress userAddress)
        {
            await _userAddressRepositoryLayer.UpdateAddressAsync(userAddress);
        }

        public async Task DeleteAddressAsync(int addressId)
        {
            await _userAddressRepositoryLayer.DeleteAddressAsync(addressId);
        }
    }
}
