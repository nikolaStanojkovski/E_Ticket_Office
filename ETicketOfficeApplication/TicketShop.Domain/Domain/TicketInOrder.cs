using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Enumerations;

namespace TicketShop.Domain.Domain
{
    public class TicketInOrder : BaseEntity
    {
        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public int Quantity { get; set; }
        public DateTime BoughtOn { get; set; }
        public TicketType TicketType { get; set; }
    }
}
