using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.CartModel;
using ModelLayer.ResponseModel;
using RepositoryLayer.Entity;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartBusinessLayer _cartBusinessLayer;

        public CartController(ICartBusinessLayer cartBusinessLayer)
        {
            _cartBusinessLayer = cartBusinessLayer;
        }

        [HttpPost()]
        public async Task<IActionResult> AddToCart([FromBody] CartModel cartModel)
        {
            var userIdClaim = User.FindFirstValue("UserId");
            int userId = Convert.ToInt32(userIdClaim);
            Cart cart = new Cart() { UserId = userId, BookId = cartModel.BookId, Quantity = cartModel.Quantity, IsWishlist = cartModel.IsWishlist};
            var result = await _cartBusinessLayer.AddToCart(cart);
            return Ok(new ResponseModel<int>
            {
                Success = true,
                Message = "Successfully Added Cart data",
                Data = result
            });
        }

        [HttpGet()]
        public async Task<IActionResult> GetCartByUserId()
        {
            var userIdClaim = User.FindFirstValue("UserId");
            int userId = Convert.ToInt32(userIdClaim);
            var result = await _cartBusinessLayer.GetCartByUserId(userId);
            return Ok(new ResponseModel<IEnumerable<CartResponse>>
            {
                Success = true,
                Message = "Successfully Cart data fetched",
                Data = result
            });
        }

        [HttpDelete()]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var userIdClaim = User.FindFirstValue("UserId");
            int userId = Convert.ToInt32(userIdClaim);
            await _cartBusinessLayer.RemoveFromCart(cartId, userId);
            return Ok("Succefully deleted");
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateCartQuantity(int cartId, int quantity)
        {
            await _cartBusinessLayer.UpdateCartQuantity(cartId, quantity);
            return NoContent();
        }
    }
}
