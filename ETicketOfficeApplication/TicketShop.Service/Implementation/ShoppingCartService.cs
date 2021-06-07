using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain;
using TicketShop.Domain.Domain;
using TicketShop.Domain.DTO;
using TicketShop.Repository.Interface;
using TicketShop.Service.Interface;

namespace TicketShop.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<TicketInShoppingCart> _ticketInShoppingCartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
        private readonly IRepository<EmailMessage> _mailRepository;
        private readonly IUserRepository _userRepository;
        private readonly EmailSettings _emailSettings;
        private readonly BackgroundEmailSender _emailSender;


        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository,
                            IRepository<TicketInShoppingCart> ticketInShoppingCartRepository,
                            IRepository<Order> orderRepository,
                            IRepository<TicketInOrder> ticketInOrderRepository,
                            IRepository<EmailMessage> mailRepository,
                            IUserRepository userRepository,
                            EmailSettings emailSettings)
        {
            this._shoppingCartRepository = shoppingCartRepository;
            this._userRepository = userRepository;
            this._ticketInShoppingCartRepository = ticketInShoppingCartRepository;
            this._orderRepository = orderRepository;
            this._ticketInOrderRepository = ticketInOrderRepository;
            this._mailRepository = mailRepository;
            this._emailSettings = emailSettings;
            this._emailSender = new BackgroundEmailSender(new EmailService(_emailSettings), _mailRepository);
        }

        public ShoppingCartDto GetShoppingCartInfo(string userId)
        {
            var loggedInUser = _userRepository.ReadUser(userId);

            var userShoppingCart = loggedInUser.Cart;

            var ticketsPrice = userShoppingCart.Tickets.Select(z => new
            {
                TicketPrice = z.Ticket.Price,
                Quantity = z.Quantity
            }).ToList();

            var total = 0.0;

            foreach (var item in ticketsPrice)
            {
                total += item.TicketPrice * item.Quantity;
            }

            ShoppingCartDto model = new ShoppingCartDto
            {
                Tickets = userShoppingCart.Tickets.ToList(),
                TotalPrice = total
            };

            return model;
        }

        public bool DeleteTicketFromShoppingCart(string userId, Guid ticketId)
        {
            var loggedInUser = _userRepository.ReadUser(userId);

            var userShoppingCart = loggedInUser.Cart;

            _ticketInShoppingCartRepository.Delete(userShoppingCart.
                Tickets.Where(z => z.TicketId == ticketId).FirstOrDefault());

            return userShoppingCart.Tickets.Where(z => z.TicketId == ticketId).FirstOrDefault() == null;
        }

        public async Task<bool> OrderFromShoppingCartAsync(string userId)
        {
            var loggedInUser = _userRepository.ReadUser(userId);
            var userShoppingCart = loggedInUser.Cart;

            EmailMessage message = new EmailMessage();
            message.MailTo = loggedInUser.Email;
            message.Subject = "Successfully created order";
            message.Status = false;

            Order item = new Order
            {
                Id = Guid.NewGuid(),
                User = loggedInUser,
                UserId = userId,
            };

            _orderRepository.Create(item);

            List<TicketInOrder> ticketInOrders = userShoppingCart.Tickets
                .Select(z => new TicketInOrder
                {
                    Id = Guid.NewGuid(),
                    OrderId = item.Id,
                    TicketId = z.Ticket.Id,
                    Ticket = z.Ticket,
                    Order = item,
                    Quantity = z.Quantity,
                    TicketType = z.TicketType,
                    BoughtOn = DateTime.Now,
                }).ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Your request for ordering tickets is completed. The order contains the following tickets: ");
            sb.AppendLine();

            var totalPrice = 0.0;
            for (int i = 0; i < ticketInOrders.Count; i++)
            {
                var ticket = ticketInOrders[i];
                totalPrice += ticket.Ticket.Price * ticket.Quantity;
                sb.AppendLine(ticket.Ticket.MovieName + " on date " + ticket.Ticket.Date + ", with ticket type of " + ticket.TicketType + ", and with quantity of " + ticket.Quantity + " and price " + ticket.Ticket.Price + " €");
            }

            sb.AppendLine("-------------------------------------------------------------------");
            sb.AppendLine("The total price of the order is " + totalPrice.ToString() + " EUR");
            message.Content = sb.ToString();

            _mailRepository.Create(message);

            foreach (var ticket in ticketInOrders)
            {
                _ticketInOrderRepository.Create(ticket);
            }

            _ticketInShoppingCartRepository.DeleteAll(_ticketInShoppingCartRepository.ReadAll());

            await _emailSender.DoWork();

            return _ticketInOrderRepository.ReadAll().Count == 0;
        }
    }
}
