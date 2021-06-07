using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain.Domain;

namespace TicketShop.Service.Interface
{
    public interface IEmailService
    {
        public Task SendEmailAsync(List<EmailMessage> emails);
    }
}
