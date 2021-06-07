using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Identity;

namespace TicketShop.Domain.Domain
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public EShopUser User { get; set; }
        public virtual ICollection<TicketInOrder> Tickets { get; set; }
    }
}
