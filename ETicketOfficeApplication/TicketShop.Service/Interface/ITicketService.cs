using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Domain;
using TicketShop.Domain.DTO;
using TicketShop.Domain.Enumerations;

namespace TicketShop.Service.Interface
{
    public interface ITicketService
    {
        public List<Ticket> GetAllTickets();
        public List<Ticket> FilterTicketsByDate();
        public List<Ticket> FilterTicketsByGenre(Genre genre);

        public void CreateTicket(Ticket p);
        public Ticket ReadTicket(Guid? id);
        public void UpdateTicket(Ticket p);
        public bool DeleteTicket(Guid id);

        public AddToShoppingCartDto GetShoppingCartInfo(Guid? id);
        public bool AddTicketToShoppingCart(string userId, AddToShoppingCartDto item);
    }
}
