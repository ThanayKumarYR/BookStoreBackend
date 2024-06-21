using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.CartModel
{
    public class CartResponse
    {
        public int CartId { get; set; }
        public int BookId { get; set; }
        //public int Quantity { get; set; }
        //public bool IsWishlist { get; set; }
        public string ImgSrc { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        //public double Rating { get; set; }
        //public int RatingPeopleCount { get; set; }
        //public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        //public bool OutOfStock { get; set; }
        //public string Description { get; set; }
    }
}
