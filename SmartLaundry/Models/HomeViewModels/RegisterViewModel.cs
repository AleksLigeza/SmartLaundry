using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "The {0} field is not a valid e-mail address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [MaxLength(40, ErrorMessage = "The field {0} must be at max {1} characters long.")]
        [RegularExpression("^[a-zA-Z ąćęłńóśźżĄĘŁŃÓŚŹŻ]*$", ErrorMessage = "The field {0} is not valid.")]
        [DataType(DataType.Text)]
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [MaxLength(40, ErrorMessage = "The field {0} must be at max {1} characters long.")]
        [RegularExpression("^[a-zA-Z ąćęłńóśźżĄĘŁŃÓŚŹŻ]*$", ErrorMessage = "The field {0} is not valid.")]
        [DataType(DataType.Text)]
        [Display(Name = "Lastname")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}