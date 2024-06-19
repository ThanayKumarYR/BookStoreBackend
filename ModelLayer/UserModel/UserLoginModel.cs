﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.UserModel
{
    public class UserLoginModel
    {
        [Required]
        public string? EmailId { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
