using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Domain;
using TicketShop.Domain.Enumerations;

namespace TicketShop.Domain.DTO
{
    public class AddToShoppingCartDto
    {
        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int Quantity { get; set; }
        public TicketType TicketType { get; set; }
    }
}
