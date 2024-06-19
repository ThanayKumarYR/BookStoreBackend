using System;
using System.ComponentModel.DataAnnotations;

namespace RepositoryLayer.Entity
{
    public class Book
    {
        [Key]
        public int BookId { get; set; } // Primary key

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(255)]
        public string Author { get; set; }

        [Required]
        public double Rating { get; set; }

        [Required]
        public int RatingPeopleCount { get; set; } // Number of people who rated

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discounted price must be greater than 0")]
        public decimal DiscountedPrice { get; set; }

        [Required]
        public bool OutOfStock { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }
    }
}
