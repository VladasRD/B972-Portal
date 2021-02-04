using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Box.Adm.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage="This field is required")]
        [EmailAddress(ErrorMessage="This is not a valid e-mail address")]
        public string Email { get; set; }

        [Required(ErrorMessage="This field is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
