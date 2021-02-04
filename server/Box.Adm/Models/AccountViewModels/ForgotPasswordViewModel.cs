using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Box.Adm.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage="This field is required")]
        [EmailAddress(ErrorMessage="This is not a valid e-mail address")]
        public string Email { get; set; }
    }
}
