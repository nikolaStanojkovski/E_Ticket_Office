using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Enumerations;

namespace TicketShop.Domain.Domain
{
    public class Ticket : BaseEntity
    {
        [Required]
        [Display(Name = "Theater Name")]
        public String TheaterName { get; set; }

        [Required]
        [Display(Name = "Movie Name")]
        public String MovieName { get; set; }

        [Required]
        [Display(Name = "Ticket Price")]
        public Double Price { get; set; }

        [Required]
        [Display(Name = "Movie Genre")]
        public Genre Genre { get; set; }

        [Required]
        [Display(Name = "Movie Date")]
        public DateTime Date { get; set; }

        public virtual ICollection<TicketInShoppingCart> ShoppingCarts { get; set; }
        public virtual ICollection<TicketInOrder> Orders { get; set; }
    }
}
