using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketShop.Domain.Domain;
using TicketShop.Domain.DTO;
using TicketShop.Domain.Enumerations;
using TicketShop.Repository;
using TicketShop.Service.Interface;

namespace TicketShop.Web.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            this._ticketService = ticketService;
        }

        // GET: Tickets
        [AllowAnonymous]
        public IActionResult Index(string error = "None")
        {
            ViewBag.Error = error;
            return View(_ticketService.GetAllTickets());
        }

        // -------------------------------------------
        // ------------------CRUD---------------------
        // -------------------------------------------

        // GET: Tickets/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create([Bind("TheaterName,MovieName,Price,Genre,Date")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _ticketService.CreateTicket(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View(_ticketService.ReadTicket(id));
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View(_ticketService.ReadTicket(id));
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(Guid id, [Bind("TheaterName,MovieName,Price,Genre,Date,Id")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _ticketService.UpdateTicket(ticket);
                return RedirectToAction(nameof(Index));
            }

            return View(ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View(_ticketService.ReadTicket(id));
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _ticketService.DeleteTicket(id);
            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------
        // -----------------OTHER---------------------
        // -------------------------------------------

        public IActionResult AddTicketToCart(Guid? id)
        {
            if (id == null)
                return NotFound();

            var model = this._ticketService.GetShoppingCartInfo(id);

            if (model == null)
                return View(new ShoppingCartDto());

            return View(model);
        }

        [HttpPost]
        public IActionResult AddTicketToCart([Bind("TicketId", "TicketType", "Quantity")] AddToShoppingCartDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._ticketService.AddTicketToShoppingCart(userId, model);

            if (result)
                return RedirectToAction("Index", "Tickets");

            return View(model);
        }

        public IActionResult FilterByDate()
        {
            return View("Index", _ticketService.FilterTicketsByDate());
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult ExportTickets([Bind("genre")] Genre genre)
        {
            List<Ticket> filteredTickets = _ticketService.FilterTicketsByGenre(genre);

            var fileName = "Tickets_Genre_" + genre.ToString() + ".xlsx";
            var contentType = "application/vnd.ms-excel";

            if (filteredTickets.Count == 0)
                return RedirectToAction("Index", "Tickets", new { error = "There are no tickets with the specified genre" });

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Tickets_" + genre.ToString());

                worksheet.Cell(1, 1).Value = "Ticket ID";
                worksheet.Cell(1, 2).Value = "Theater Name";
                worksheet.Cell(1, 3).Value = "Movie Name";
                worksheet.Cell(1, 4).Value = "Price (EUR)";
                worksheet.Cell(1, 5).Value = "Date";

                for (int i = 1; i <= filteredTickets.Count; i++)
                {
                    var item = filteredTickets[i - 1];
                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.TheaterName;
                    worksheet.Cell(i + 1, 3).Value = item.MovieName;
                    worksheet.Cell(i + 1, 4).Value = item.Price.ToString();
                    worksheet.Cell(i + 1, 5).Value = item.Date.ToString();
                }

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, contentType, fileName);
            }
        }

    }
}
