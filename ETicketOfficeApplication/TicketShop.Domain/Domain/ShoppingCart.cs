using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Identity;

namespace TicketShop.Domain.Domain
{
    public class ShoppingCart : BaseEntity
    {
        public string OwnerId { get; set; }
        public virtual EShopUser Owner { get; set; }
        public virtual ICollection<TicketInShoppingCart> Tickets { get; set; }
    }
}
