using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Domain;

namespace TicketShop.Service.Interface
{
    public interface IOrderService
    {
        public List<Order> ReadAllOrders();

        public List<Order> ReadAllOrdersForUser(string userId);

        public Order ReadOrder(Guid? id);

        public void DeleteOrder(Guid? id);
    }
}
