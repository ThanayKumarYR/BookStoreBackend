using System.Collections.Generic;
using System.Threading.Tasks;
using ModelLayer.CartModel;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface ICartBusinessLayer
    {
        Task<int> AddToCart(Cart cart);
        Task<IEnumerable<CartResponse>> GetCartByUserId(int userId);
        Task RemoveFromCart(int cartId, int userId);
        Task UpdateCartQuantity(int cartId, int quantity);
    }
}
