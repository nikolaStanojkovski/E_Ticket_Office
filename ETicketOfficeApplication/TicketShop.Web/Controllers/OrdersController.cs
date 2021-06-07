using GemBox.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TicketShop.Service.Interface;

namespace TicketShop.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            this._orderService = orderService;

            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        // GET Filtered Orders: Orders
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(_orderService.ReadAllOrdersForUser(userId));
        }

        // GET Invoice: Orders/GenerateInvoice/5
        public FileContentResult GenerateInvoice(Guid? ticketId)
        {
            var order = _orderService.ReadOrder(ticketId);

            var templatePath = Path.Combine($"{Directory.GetCurrentDirectory()}\\Files", "Invoice.docx");
            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{OrderNumber}}", order.Id.ToString());
            document.Content.Replace("{{UserName}}", order.User.UserName);

            StringBuilder sb = new StringBuilder();
            var totalPrice = 0.0;

            foreach (var item in order.Tickets)
            {
                totalPrice += item.Ticket.Price * item.Quantity;
                sb.AppendLine(item.Ticket.MovieName + " on date " + item.Ticket.Date + ", with ticket type of " + item.TicketType + ", and with quantity of " + item.Quantity + " and price " + item.Ticket.Price + " €");
                sb.AppendLine();
            }

            document.Content.Replace("{{TicketList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString() + "€ (EUR)");

            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "Invoice_Order_" + order.Id + ".pdf");
        }
    }
}
