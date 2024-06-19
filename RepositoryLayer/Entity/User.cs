using System.ComponentModel.DataAnnotations;

namespace RepositoryLayer.Entity
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string? FullName { get; set; }
        [Required]
        public string? EmailId { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? MobileNumber { get; set; }

    }
}
