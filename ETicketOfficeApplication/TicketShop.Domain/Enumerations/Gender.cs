using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketShop.Domain.Enumerations
{
    public enum Gender
    {
        [Display(Name = "Male")]
        MALE,
        [Display(Name = "Female")]
        FEMALE,
        [Display(Name = "Other")]
        OTHER
    }
}
