using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.DTO;

namespace TicketShop.Service.Interface
{
    public interface IShoppingCartService
    {
        public ShoppingCartDto GetShoppingCartInfo(string userId);
        public bool DeleteTicketFromShoppingCart(string userId, Guid ticketId);
        public Task<bool> OrderFromShoppingCartAsync(string userId);
    }
}
