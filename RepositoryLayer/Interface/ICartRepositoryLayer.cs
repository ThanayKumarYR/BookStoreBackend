using System.Collections.Generic;
using System.Threading.Tasks;
using ModelLayer.CartModel;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface ICartRepositoryLayer
    {
        Task<int> AddToCart(Cart cart);
        Task<IEnumerable<CartResponse>> GetCartByUserId(int userId);
        Task RemoveFromCart(int cartId, int userId);
        Task UpdateCartQuantity(int cartId, int quantity);
    }
}
