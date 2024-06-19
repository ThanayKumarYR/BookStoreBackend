using System;
using System.ComponentModel.DataAnnotations;

namespace RepositoryLayer.Entity
{
    public class User
    {
        [Required]
        public string? FullName { get; set; }
        public string? EmailId { get; set; }
        public string? Password { get; set; }
        public string? MobileNumber { get; set; }
    }
}
