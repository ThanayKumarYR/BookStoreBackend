using System;
using System.ComponentModel.DataAnnotations;

namespace RepositoryLayer.Entity
{
    public class UserAddress
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public string? City { get; set; }

        [Required]
        public string? State { get; set; }

        [Required]
        public string? Type { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
