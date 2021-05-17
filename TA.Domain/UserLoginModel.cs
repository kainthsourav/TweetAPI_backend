using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TA.Domain
{
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string email { get; set; }


        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }
    }
}
