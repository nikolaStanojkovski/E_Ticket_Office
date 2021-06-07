using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Identity;

namespace TicketShop.Domain.DTO
{
    public class AddUserToRoleDTO
    {
        public string UserMail { get; set; }
        public string Role { get; set; }

        public List<string> Roles { get; set; }
        public List<string> Users { get; set; }
    }
}
