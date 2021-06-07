using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Enumerations;

namespace TicketShop.Domain.Identity
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "Your first name is required")]
        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public String MiddleName { get; set; }

        [Required(ErrorMessage = "Your last name is required")]
        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        [Required(ErrorMessage = "Your address is required")]
        [Display(Name = "Address")]
        public String Address { get; set; }

        [Required(ErrorMessage = "Your age is required")]
        [Range(1, int.MaxValue)]
        public int Age { get; set; }

        [Required(ErrorMessage = "Your gender is required")]
        public Gender Gender { get; set; }



        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [Compare("Password", ErrorMessage = "The password and Confirm password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
