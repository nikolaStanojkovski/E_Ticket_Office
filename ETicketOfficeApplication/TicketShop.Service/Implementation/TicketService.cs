using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Domain;
using TicketShop.Domain.DTO;
using TicketShop.Domain.Enumerations;
using TicketShop.Repository.Interface;
using TicketShop.Service.Interface;

namespace TicketShop.Service.Implementation
{
    public class TicketService : ITicketService
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<TicketInShoppingCart> _ticketInShoppingCartRepository;
        private readonly IUserRepository _userRepository;

        public TicketService(IRepository<Ticket> ticketRepository, 
            IUserRepository userRepository, 
            IRepository<TicketInShoppingCart> ticketInShoppingCartRepository)
        {
            this._ticketRepository = ticketRepository;
            this._ticketInShoppingCartRepository = ticketInShoppingCartRepository;
            this._userRepository = userRepository;
        }

        public void CreateTicket(Ticket ticket)
        {
            ticket.Id = Guid.NewGuid();
            this._ticketRepository.Create(ticket);
        }

        public Ticket ReadTicket(Guid? id)
        {
            return this._ticketRepository.Read(id);
        }

        public void UpdateTicket(Ticket ticket)
        {
            if (ticket != null)
            {
                this._ticketRepository.Update(ticket);
            }
        }

        public bool DeleteTicket(Guid id)
        {
            var ticket = this.ReadTicket(id);
            if (ticket == null)
                return false;

            this._ticketRepository.Delete(ticket);
            return true;
        }


        public List<Ticket> GetAllTickets()
        {
            return _ticketRepository.ReadAll();
        }

        public AddToShoppingCartDto GetShoppingCartInfo(Guid? id)
        {
            var ticket = this.ReadTicket(id);
            if (ticket == null)
                return null;

            AddToShoppingCartDto item = new AddToShoppingCartDto
            {
                TicketId = ticket.Id,
                Ticket = ticket,
                Quantity = 1
            };

            return item;
        }

        public bool AddTicketToShoppingCart(string userId, AddToShoppingCartDto item)
        {
            if (item.TicketId != null && userId != null)
            {
                var loggedUser = this._userRepository.ReadUser(userId);

                var userShoppingCart = loggedUser.Cart;

                if (item.TicketId != null && userShoppingCart != null)
                {
                    var ticket = this.ReadTicket(item.TicketId);

                    if (ticket != null)
                    {
                        if (userShoppingCart.Tickets.Where(z => z.Ticket.Id == ticket.Id && z.TicketType.Equals(item.TicketType)).Count() != 0)
                        {
                            var existingTicket = userShoppingCart.Tickets.Where(z => z.TicketId == ticket.Id).FirstOrDefault();
                            existingTicket.Quantity = existingTicket.Quantity + item.Quantity;

                            _ticketInShoppingCartRepository.Update(existingTicket);
                        }
                        else
                        {
                            TicketInShoppingCart model = new TicketInShoppingCart
                            {
                                Id = Guid.NewGuid(),
                                Ticket = ticket,
                                TicketId = ticket.Id,
                                CartId = userShoppingCart.Id,
                                Cart = userShoppingCart,
                                TicketType = item.TicketType,
                                BoughtOn = DateTime.Now,
                                Quantity = item.Quantity
                            };

                            _ticketInShoppingCartRepository.Create(model);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public List<Ticket> FilterTicketsByDate()
        {
            return GetAllTickets().Where(z =>
                DateTime.Compare(z.Date, DateTime.Now) > 0).ToList();
        }

        public List<Ticket> FilterTicketsByGenre(Genre genre)
        {
            return GetAllTickets().Where(z => z.Genre.Equals(genre)).ToList();
        }
    }
}
