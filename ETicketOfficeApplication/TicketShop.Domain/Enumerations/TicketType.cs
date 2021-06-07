using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketShop.Domain.Enumerations
{
    public enum TicketType
    {
        [Display(Name = "First Level")]
        FIRST_LEVEL_SEATS,
        [Display(Name = "Mid Level")]
        MID_LEVEL_SEATS,
        [Display(Name = "Last Level")]
        LAST_LEVEL_SEATS,
        [Display(Name = "Standing Level")]
        STANDING_LEVEL
    }
}
