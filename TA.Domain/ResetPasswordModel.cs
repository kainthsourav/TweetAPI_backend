using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TA.Domain
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }

        public string email { get; set; }
        //public string token { get; set; }
    }
}
