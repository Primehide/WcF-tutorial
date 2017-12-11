using SC.BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC.BL
{
    public interface ITicketManager
    {
        IEnumerable<Ticket> GetTickets();
        Ticket AddTicket(int accountId, string question);
        Ticket AddTicket(int accountId, string device, string problem);
        Ticket GetTicket(int nbr);

        void ChangeTicket(Ticket ticket);
        void RemoveTicket(int ticketNumber);
        void ChangeStateToClosed(int ticketNumber);

        IEnumerable<TicketResponse> GetTicketResponses(int ticketNumber);
        TicketResponse AddTicketResponse(int ticketNumber, string response, bool isClientResponse);
    }
}