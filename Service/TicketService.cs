using System;
using System.Collections.Generic;
using System.Linq;
using SC.BL.Domain;
using SC.DAL;
using SC.DAL.EF;
using Service.Models;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.ServiceModel;

namespace Service
{
    public class TicketService : ITicket, ITicketREST
    {
        private ITicketRepository repo;
        public TicketService()
        {
            repo = new TicketRepository();
        }

        public TicketResponse AddTicketResponse(int ticketNumber, string response, bool isClientResponse)
        {
            Ticket ticketToAddResponseTo = repo.ReadTicket(ticketNumber);
            //ticketToAddResponseTo.Responses = new List<TicketResponse>();
            TicketResponse newTicketResponse = new TicketResponse();
            newTicketResponse.Date = DateTime.Now;
            newTicketResponse.Text = response;
            newTicketResponse.IsClientResponse = isClientResponse;
            newTicketResponse.Ticket = ticketToAddResponseTo;
            repo.CreateTicketResponse(newTicketResponse);
            return newTicketResponse;
        }

        public void CloseTicket(int ticketnumber)
        {
            repo.UpdateTicketStateToClosed(ticketnumber);
        }

        public Ticket CreateTicket(int accountId, string question)
        {
            Ticket ticketToCreate = new Ticket()
            {
                AccountId = accountId,
                Text = question,
                DateOpened = DateTime.Now,
                State = TicketState.Open
            };
            return repo.CreateTicket(ticketToCreate);
        }

        public Ticket CreateTicket(int accountId, string device, string problem)
        {
            HardwareTicket hwTicketToAdd = new HardwareTicket()
            {
                AccountId = accountId,
                DateOpened = System.DateTime.Now,
                DeviceName = device,
                State = TicketState.Open,
                Text = problem
            };
            return repo.CreateTicket(hwTicketToAdd);
        }

        public HttpResponseMessage CreateTicketREST(TicketDTO ticket)
        {
            Ticket ticketToCreate = new Ticket()
            {
                AccountId = ticket.AccountId,
                Text = ticket.Question,
                DateOpened = DateTime.Now,
                State = TicketState.Open
            };
            repo.CreateTicket(ticketToCreate);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        public IEnumerable<HardwareTicket> GetHwTickets()
        {
            List<HardwareTicket> HwTickets = repo.ReadHwTickets().ToList();
            return HwTickets;
        }

        public Ticket GetTicket(int id)
        {
            return repo.ReadTicket(id);
        }

        public IEnumerable<TicketResponse> GetTicketResponses(int ticketNumber)
        {
            return repo.ReadTicketResponsesOfTicket(ticketNumber);
        }

        public IEnumerable<Ticket> GetTickets()
        {
            return repo.ReadNormalTickets();
        }

        public IEnumerable<Ticket> GetTicketsREST()
        {
            return repo.ReadNormalTickets();
        }

        public string SayHello()
        {
            return string.Format("Hello");
        }
    }
}
