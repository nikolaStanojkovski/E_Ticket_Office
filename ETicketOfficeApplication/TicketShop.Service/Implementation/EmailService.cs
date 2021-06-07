using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Domain;
using TicketShop.Domain.Domain;
using TicketShop.Repository.Interface;
using TicketShop.Service.Interface;

namespace TicketShop.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(EmailSettings settings)
        {
            this._settings = settings;
        }

        public async Task SendEmailAsync(List<EmailMessage> emails)
        {
            List<MimeMessage> messages = new List<MimeMessage>();

            foreach (var item in emails)
            {
                var message = new MimeMessage
                {
                    Sender = new MailboxAddress(_settings.SenderName, _settings.SMTPUsername),
                    Subject = item.Subject
                };

                message.From.Add(new MailboxAddress(_settings.EmailDisplayName, _settings.SMTPUsername));

                message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                {
                    Text = item.Content
                };

                message.To.Add(new MailboxAddress(item.MailTo));

                messages.Add(message);
            }

            try
            {
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    var socketOptions = _settings.EnableSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

                    await smtp.ConnectAsync(_settings.SMTPServer, _settings.SMTPServerPort, socketOptions);

                    if (!string.IsNullOrEmpty(_settings.SMTPUsername))
                    {
                        await smtp.AuthenticateAsync(_settings.SMTPUsername, _settings.SMTPPassword);
                    }

                    foreach (var item in messages)
                    {
                        await smtp.SendAsync(item);
                    }

                    await smtp.DisconnectAsync(true);
                }
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
        }
    }
}
