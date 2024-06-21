using System.ComponentModel.DataAnnotations;

namespace RepositoryLayer.Entity
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public bool IsWishlist { get; set; }
    }
}
