using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Identity;
using TicketShop.Repository.Interface;

namespace TicketShop.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<EShopUser> entities;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<EShopUser>();
        }

        public List<EShopUser> GetAll()
        {
            return entities.ToListAsync().Result;
        }

        public List<string> GetAllMails()
        {
            List<EShopUser> users = GetAll();
            List<string> mails = new List<string>();
            foreach(var user in users)
            {
                mails.Add(user.Email);
            }

            return mails;
        }

        public EShopUser ReadUser(string id)
        {
            return entities
                .Include(z => z.Cart)
                .Include(z => z.Cart.Tickets)
                .Include("Cart.Tickets.Ticket")
                .SingleOrDefaultAsync(z => z.Id.Equals(id)).Result;
        }

        public void CreateUser(EShopUser entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Add(entity);
            context.SaveChanges();
        }

        public void UpdateUser(EShopUser entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Update(entity);
            context.SaveChanges();
        }

        public void DeleteUser(EShopUser entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
