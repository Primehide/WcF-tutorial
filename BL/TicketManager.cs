using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC.BL.Domain;
using System.ComponentModel.DataAnnotations;

namespace SC.BL
{
    public class TicketManager : ITicketManager
    {
        private DAL.EF.TicketRepository repo;

        public TicketManager()
        {
            //repo = new DAL.SqlClient.TicketRepository();
            repo = new DAL.EF.TicketRepository();
        }

        public Ticket AddTicket(int accountId, string question)
        {
            Ticket ticketToAdd = new Ticket()
            {
                AccountId = accountId,
                DateOpened = System.DateTime.Now,
                State = TicketState.Open,
                Text = question,
            };
            Validate(ticketToAdd);
            return repo.CreateTicket(ticketToAdd);
        }

        public Ticket AddTicket(int accountId, string device, string problem)
        {
            HardwareTicket hwTicketToAdd = new HardwareTicket()
            {
                AccountId = accountId,
                DateOpened = System.DateTime.Now,
                DeviceName = device,
                State = TicketState.Open,
                Text = problem
            };
            Validate(hwTicketToAdd);
            return repo.CreateTicket(hwTicketToAdd);
        }

        public TicketResponse AddTicketResponse(int ticketNumber, string response, bool isClientResponse)
        {
            Ticket ticketToAddResponseTo = this.GetTicket(ticketNumber);
            if (ticketToAddResponseTo != null)
            {
                // Create response
                TicketResponse newTicketResponse = new TicketResponse();
                newTicketResponse.Date = DateTime.Now;
                newTicketResponse.Text = response;
                newTicketResponse.IsClientResponse = isClientResponse;
                newTicketResponse.Ticket = ticketToAddResponseTo;

                // Add response to ticket
                var responses = this.GetTicketResponses(ticketNumber);
                //If the responses are loaded, check if they exisist. 
                // If so, make sure that we have a List<T>, otherwise create a new List<T>
                if (responses != null)
                    ticketToAddResponseTo.Responses = responses.ToList();
                else
                    ticketToAddResponseTo.Responses = new List<TicketResponse>();

                ticketToAddResponseTo.Responses.Add(newTicketResponse);

                // Change state of ticket, depending of who has answered it
                ticketToAddResponseTo.State = isClientResponse ? TicketState.ClientAnswer: TicketState.Answered;

                // Before saving the changes to the repository, check is all validation logic
                // is valid
                this.Validate(newTicketResponse);
                this.Validate(ticketToAddResponseTo);

                // Save changes to repository
                repo.CreateTicketResponse(newTicketResponse);
                repo.UpdateTicket(ticketToAddResponseTo);

                return newTicketResponse;
            }
            else
                throw new ArgumentException("Ticketnumber '" + ticketNumber + "' not found!");
        }

        public void ChangeStateToClosed(int ticketNumber)
        {
            repo.UpdateTicketStateToClosed(ticketNumber);
        }

        public void ChangeTicket(Ticket ticketToUpdate)
        {
            Validate(ticketToUpdate);
            repo.UpdateTicket(ticketToUpdate);
        }

        public Ticket GetTicket(int nbr)
        {
            return repo.ReadTicket(nbr);
        }

        public IEnumerable<TicketResponse> GetTicketResponses(int ticketNumber)
        {
            return repo.ReadTicketResponsesOfTicket(ticketNumber);
        }

        public IEnumerable<Ticket> GetTickets()
        {
            return repo.ReadTickets();
        }

        public void RemoveTicket(int ticketNumber)
        {
            repo.DeleteTicket(ticketNumber);
        }

        private void Validate(Ticket ticket)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            bool valid = Validator.TryValidateObject(ticket, new ValidationContext(ticket)
                                                     , errors, validateAllProperties: true);

            if (!valid)
                throw new ValidationException("Ticket not valid!");
        }

        private void Validate(TicketResponse response)
        {
            Validator.ValidateObject(response, new ValidationContext(response), validateAllProperties: true);
        }


    }
}
