using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Domain;
using TicketShop.Repository.Interface;

namespace TicketShop.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<Order> entities;

        public OrderRepository(ApplicationDbContext context)
        {
            this._context = context;
            this.entities = context.Set<Order>();
        }

        public List<Order> GetAllOrders()
        {
            return this.entities
                .Include(z => z.Tickets)
                .Include(z => z.User)
                .Include("Tickets.Ticket")
                .ToListAsync().Result;
        }

        public Order ReadOrder(Guid? id)
        {
            return this.entities
                .Include(z => z.Tickets)
                .Include(z => z.User)
                .Include("Tickets.Ticket")
                .SingleOrDefaultAsync(z => z.Id.Equals(id)).Result;
        }

        public void DeleteOrder(Guid? id)
        {
            if (id == null)
                throw new ArgumentNullException("entity");

            this.entities.Remove(this.entities.SingleOrDefault(z => z.Id.Equals(id)));
            _context.SaveChanges();
        }
    }
}
