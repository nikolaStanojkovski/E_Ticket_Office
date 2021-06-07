using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketShop.Service.Interface;

namespace TicketShop.Web.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            this._shoppingCartService = shoppingCartService;
        }

        // GET: Shopping Cart Info
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _shoppingCartService.GetShoppingCartInfo(userId);

            return View(model);
        }

        // GET: Creating and order and payment
        public IActionResult PayOrder(string stripeEmail, string stripeToken)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = _shoppingCartService.GetShoppingCartInfo(userId);

            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(order.TotalPrice) * 100,
                Description = "E Ticket Office Application Payment",
                Currency = "eur",
                Customer = customer.Id
            });

            if (charge.Status == "succeeded")
            {
                var result = this.Order();
                return result ? RedirectToAction("Index", "Tickets") : RedirectToAction("Index", "ShoppingCart");
            }

            return RedirectToAction("Error", "Home");

        }

        // DELETE: Ticket from shopping cart
        public IActionResult Delete(Guid ticketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = _shoppingCartService.DeleteTicketFromShoppingCart(userId, ticketId);

            return result ? RedirectToAction("Index", "ShoppingCart") : RedirectToAction("Index", "Tickets");
        }

        // ORDER: Creating an order from the shopping cart
        private bool Order()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = _shoppingCartService.OrderFromShoppingCartAsync(userId);

            return result.Result;
        }
    }
}
