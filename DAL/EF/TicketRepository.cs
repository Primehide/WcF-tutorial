using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC.BL.Domain;
using System.Data.Entity;

namespace SC.DAL.EF
{
    public class TicketRepository : ITicketRepository
    {
        private SupportCenterDbContext ctx = null;

        public TicketRepository()
        {
            ctx = new SupportCenterDbContext();
            //ctx.Database.Initialize(false);
        }

        public Ticket CreateTicket(Ticket ticketToCreate)
        {
            ctx.Tickets.Add(ticketToCreate);
            ctx.SaveChanges();

            return ticketToCreate;
        }

        public TicketResponse CreateTicketResponse(TicketResponse response)
        {
            ctx.TicketResponses.Add(response);
            ctx.SaveChanges();

            return response;
        }

        public void DeleteTicket(int ticketNumber)
        {
            Ticket ticket = ctx.Tickets.Find(ticketNumber);
            ctx.Tickets.Remove(ticket);
            ctx.SaveChanges();
        }

        public IEnumerable<HardwareTicket> ReadHwTickets()
        {
            return ctx.HardwareTickets.Include(t => t.Responses).AsEnumerable();
        }

        public IEnumerable<Ticket> ReadNormalTickets()
        {
            return ctx.Tickets.Include(t => t.Responses).Where(t => !(t is HardwareTicket)).AsEnumerable();
        }

        public Ticket ReadTicket(int nbr)
        {
            Ticket ticket = ctx.Tickets.Include(t => t.Responses).Single(t => t.TicketNumber == nbr);
            return ticket;
        }

        public IEnumerable<TicketResponse> ReadTicketResponsesOfTicket(int ticketNumber)
        {
            IEnumerable<TicketResponse> responses = ctx.TicketResponses.Where(r => r.Ticket.TicketNumber == ticketNumber).AsEnumerable<TicketResponse>();
            return responses;
        }

        public IEnumerable<Ticket> ReadTickets()
        {
            IEnumerable<Ticket> tickets = ctx.Tickets.AsEnumerable();
            return tickets;
        }

        public void UpdateTicket(Ticket ticket)
        {
            //Eerst nakijken of het ticket bekend is in de context
            //en dat de status modified is voor het updaten in de DB
            ctx.Entry(ticket).State = EntityState.Modified;
            ctx.SaveChanges();
        }

        public void UpdateTicketStateToClosed(int ticketnumber)
        {
            Ticket ticket = ctx.Tickets.Find(ticketnumber);
            ticket.State = TicketState.Closed;
            ctx.SaveChanges();
        }
    }
}
