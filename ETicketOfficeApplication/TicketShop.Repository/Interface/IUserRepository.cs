using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Identity;

namespace TicketShop.Repository.Interface
{
    public interface IUserRepository
    {
        List<EShopUser> GetAll();

        List<string> GetAllMails();

        // For one entity

        void CreateUser(EShopUser entity);

        EShopUser ReadUser(string? id);

        void UpdateUser(EShopUser entity);

        void DeleteUser(EShopUser entity);
    }
}
