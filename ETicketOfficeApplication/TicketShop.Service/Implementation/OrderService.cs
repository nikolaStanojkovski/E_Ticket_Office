using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Domain;
using TicketShop.Repository.Interface;
using TicketShop.Service.Interface;

namespace TicketShop.Service.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }

        public List<Order> ReadAllOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        public List<Order> ReadAllOrdersForUser(string userId)
        {
            return this.ReadAllOrders().Where(z => z.UserId.Equals(userId)).ToList();
        }

        public Order ReadOrder(Guid? id)
        {
            return _orderRepository.ReadOrder(id);
        }

        public void DeleteOrder(Guid? id)
        {
            this._orderRepository.DeleteOrder(id);
        }
    }
}
