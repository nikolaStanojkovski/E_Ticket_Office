using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Domain;

namespace TicketShop.Repository.Interface
{
    public interface IOrderRepository
    {
        List<Order> GetAllOrders();

        Order ReadOrder(Guid? id);

        void DeleteOrder(Guid? id);
    }
}
