using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Domain;
using TicketShop.Domain.Enumerations;

namespace TicketShop.Domain.Identity
{
    public class EShopUser : IdentityUser
    {
        public String FirstName { get; set; }

        public String MiddleName { get; set; }

        public String LastName { get; set; }

        public String Address { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }


        public virtual ShoppingCart Cart { get; set; }
    }
}
