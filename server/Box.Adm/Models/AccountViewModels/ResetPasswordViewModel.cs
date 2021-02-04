using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Box.Adm.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage="This field is required")]
        //[EmailAddress(ErrorMessage="This is not a valid e-mail address")]
        public string Email { get; set; }

        [Required(ErrorMessage="This field is required")]
        [StringLength(100, ErrorMessage = "It must be at least {2} and at max {1} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]        
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
