using SC.BL.Domain;
using System.Collections.Generic;

namespace SC.DAL
{
    public interface ITicketRepository
    {
        Ticket CreateTicket(Ticket ticketToCreate);
        IEnumerable<Ticket> ReadTickets();
        IEnumerable<Ticket> ReadNormalTickets();
        IEnumerable<HardwareTicket> ReadHwTickets();
        Ticket ReadTicket(int nbr);
        void UpdateTicket(Ticket ticket);
        void DeleteTicket(int ticketNumber);
        void UpdateTicketStateToClosed(int ticketnumber);
        IEnumerable<TicketResponse> ReadTicketResponsesOfTicket(int ticketNumber);
        TicketResponse CreateTicketResponse(TicketResponse response);
    }
}
