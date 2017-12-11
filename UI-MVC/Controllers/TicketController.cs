﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SC.BL;
using SC.BL.Domain;

namespace SC.UI.Web.MVC.Controllers
{
    public class TicketController : Controller
    {
        private ITicketManager mgr = new TicketManager();
        //private TicketClient client = new TicketClient("WSHttpBinding_ITicket");

        // GET: Ticket
        public ActionResult Index()
        {
            //IEnumerable<Ticket> tickets = client.GetTickets();
            IEnumerable<Ticket> tickets = mgr.GetTickets();
            return View(tickets);
        }

        // GET: Ticket/Details/5
        public ActionResult Details(int id)
        {
            Ticket ticket = mgr.GetTicket(id);
            //Ticket ticket = client.GetTicket(id);
            //ticket.Responses = new List<TicketResponse>(client.GetTicketResponses(id));
            ticket.Responses = new List<TicketResponse>(mgr.GetTicketResponses(id));
            // OF: via ViewBag
            ViewBag.Responses = new List<TicketResponse>(mgr.GetTicketResponses(id));
            return View(ticket);
        }

        // GET: Ticket/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ticket/Create
        [HttpPost]
        public ActionResult Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket = mgr.AddTicket(ticket.AccountId, ticket.Text);
                //ticket = client.CreateTicket(ticket.AccountId, ticket.Text);
                return RedirectToAction("Details", new { id = ticket.TicketNumber });
            }
            return View();
        }

        // GET: Ticket/Edit/5
        public ActionResult Edit(int id)
        {
            Ticket ticket = mgr.GetTicket(id);
            //Ticket ticket = client.GetTicket(id);
            return View(ticket);
        }

        // POST: Ticket/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                mgr.ChangeTicket(ticket);
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Ticket/Delete/5
        public ActionResult Delete(int id)
        {
            Ticket ticket = mgr.GetTicket(id);
            //Ticket ticket = client.GetTicket(id);
            return View(ticket);
        }

        // POST: Ticket/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                mgr.RemoveTicket(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
