using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketShop.Domain.Enumerations
{
    public enum Genre
    {
        [Display(Name = "Comedy")]
        COMEDY,
        [Display(Name = "Action")]
        ACTION,
        [Display(Name = "Horror")]
        HORROR,
        [Display(Name = "Romance")]
        ROMANCE,
        [Display(Name = "Drama")]
        DRAMA,
        [Display(Name = "Documentary")]
        DOCUMENTARY,
        [Display(Name = "Musical")]
        MUSICAL,
        [Display(Name = "Thriller")]
        THRILLER,
        [Display(Name = "Crime")]
        CRIME
    }
}
