using BusinessLayer.Interface;
using ModelLayer.CartModel;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class CartBusinessLayer : ICartBusinessLayer
    {
        private readonly ICartRepositoryLayer _cartRepositoryLayer;

        public CartBusinessLayer(ICartRepositoryLayer cartRepositoryLayer)
        {
            _cartRepositoryLayer = cartRepositoryLayer;
        }

        public async Task<int> AddToCart(Cart cart)
        {
            return await _cartRepositoryLayer.AddToCart(cart);
        }

        public async Task<IEnumerable<CartResponse>> GetCartByUserId(int userId)
        {
            return await _cartRepositoryLayer.GetCartByUserId(userId);
        }

        public async Task RemoveFromCart(int cartId, int userId)
        {
            await _cartRepositoryLayer.RemoveFromCart(cartId, userId);
        }

        public async Task UpdateCartQuantity(int cartId, int quantity)
        {
            await _cartRepositoryLayer.UpdateCartQuantity(cartId, quantity);
        }
    }
}
