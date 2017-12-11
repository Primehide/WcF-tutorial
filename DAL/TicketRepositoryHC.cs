using System;
using System.Collections.Generic;
using SC.BL.Domain;

using System.Linq;

namespace SC.DAL
{
    public class TicketRepositoryHC : ITicketRepository
    {
        private ICollection<Ticket> tickets;
        private ICollection<TicketResponse> responses;

        public TicketRepositoryHC()
        {
            tickets = new List<Ticket>();
            responses = new List<TicketResponse>();
            Seed();
        }

        private void Seed()
        {          
            // Create first ticket with three responses
            Ticket t1 = new Ticket()
            {
                TicketNumber = tickets.Count + 1,
                AccountId = 1,
                Text = "Ik kan mij niet aanmelden op de webmail",
                DateOpened = new DateTime(2012, 9, 9, 13, 5, 59),
                State = TicketState.Open,
                Responses = new List<TicketResponse>()
            };
            tickets.Add(t1);

            TicketResponse t1r1 = new TicketResponse()
            {
                Id = responses.Count + 1,
                Ticket = t1,
                Text = "Account was geblokkeerd",
                Date = new DateTime(2012, 9, 9, 13, 24, 48),
                IsClientResponse = false
            };
            t1.Responses.Add(t1r1);
            responses.Add(t1r1);

            TicketResponse t1r2 = new TicketResponse()
            {
                Id = responses.Count + 1,
                Ticket = t1,
                Text = "Account terug in orde en nieuw paswoord ingesteld",
                Date = new DateTime(2012, 9, 9, 13, 29, 11),
                IsClientResponse = false
            };
            t1.Responses.Add(t1r2);
            responses.Add(t1r2);

            TicketResponse t1r3 = new TicketResponse()
            {
                Id = responses.Count + 1,
                Ticket = t1,
                Text = "Aanmelden gelukt en paswoord gewijzigd",
                Date = new DateTime(2012, 9, 10, 7, 22, 36),
                IsClientResponse = true
            };
            t1.Responses.Add(t1r3);
            responses.Add(t1r3);
            t1.State = TicketState.Closed;

            // Create second ticket with one response
            Ticket t2 = new Ticket()
            {
                TicketNumber = tickets.Count + 1,
                AccountId = 1,
                Text = "Geen internetverbinding",
                DateOpened = new DateTime(2012, 11, 5, 9, 45, 13),
                State = TicketState.Open,
                Responses = new List<TicketResponse>()
            };
            tickets.Add(t2);
            TicketResponse t2r1 = new TicketResponse()
            {
                Id = responses.Count + 1,
                Ticket = t2,
                Text = "Controleer of de kabel goed is aangesloten",
                Date = new DateTime(2012, 11, 5, 11, 25, 42),
                IsClientResponse = false
            };
            t2.Responses.Add(t2r1);
            responses.Add(t2r1);
            t2.State = TicketState.Answered;

            // Create hardware ticket without response
            HardwareTicket ht1 = new HardwareTicket()
            {
                TicketNumber = tickets.Count + 1,
                AccountId = 2,
                Text = "Blue screen!",
                DateOpened = new DateTime(2012, 12, 14, 19, 5, 2),
                State = TicketState.Open,
                DeviceName = "PC-123456"
            };
            tickets.Add(ht1);
        }

        public Ticket CreateTicket(Ticket ticketToCreate)
        {
            //We gaan van het scenario vanuit dat de
            // databank verantwoordelijk is voor het genereren
            // van de unieke nummers van een ticket (PrimaryKey)
            //Hier simuleren we dit door de ticketnumber toe te wijzen
            //op basis van het aantal elementen in onze lijst
            // van tickets
            ticketToCreate.TicketNumber = tickets.Count + 1;
            tickets.Add(ticketToCreate);
            return ticketToCreate;
        }

        public IEnumerable<Ticket> ReadTickets()
        {
            return tickets;
        }

        public Ticket ReadTicket(int nbr)
        {
            //Via Single geven we één element terug uit de lijst van Tickets.
            // is een Linq-methode!
            return tickets.Single<Ticket>(t => t.TicketNumber == nbr);
            /*
            //Als we werken met een List<Ticket> ipv de interface ICollection, kunnen we
            // ook de find-methode van een ICollection gebruiken.
            return ((List<Ticket>)tickets).Find(t => t.TicketNumber == nbr);

            //========================================================//
            // Ter informatie:                                        //
            // HIER STAAN 4 manieren onder elkaar om een zoekoperatie //
            // op een lijst uit te voeren                             //
            //========================================================//

            List<Ticket> gevondenTicketsOldSchool = new List<Ticket>();
            foreach (Ticket ticket in this.tickets)
            {
                if (ticket.TicketNumber == nbr)
                    gevondenTicketsOldSchool.Add(ticket);
            }
            
            //LINQ is een soort van query-taal in .NET. De keywords (methodes) kunnen we opnieuw gebruiken,
            // maar deze vorm van zoeken behoort niet tot de leerstof. De hierrop volgende vormen dan weer wel uiteraard!
            var gevondenTicketsLinq
                = from ticket in tickets
                  where ticket.TicketNumber == nbr
                  select ticket;

            //Delegate gaat niet => ik kan de variable nbr niet mee doorgeven aan de methode die ik met de delegate ga oproepen...

            //Anonymous method
            var gevondenTicketAnonymousMethod = tickets.Where<Ticket>
                                    (delegate (Ticket ticket)
                                    { return ticket.TicketNumber == nbr; }
                                    );

            
            //Lambda: the holy grail!!! Kort en krachtige code, the way-to-go dus!
            var gevondenTicketsLambda
                = tickets.Where<Ticket>(ticket => ticket.TicketNumber == nbr);

            //Als we toch maar één record terug verwachten, kunnen we beter Single gebruiken (zie eerste lijn code uit deze methode)
            */
        }

        public void UpdateTicket(Ticket ticket)
        {
            // Do nothing! All data lives in memory, so everything references the same objects!!
        }

        public void DeleteTicket(int ticketNumber)
        {
            ((List<TicketResponse>)this.responses).RemoveAll(r => r.Ticket.TicketNumber == ticketNumber);
            this.tickets.Remove(ReadTicket(ticketNumber));

        }

        public IEnumerable<TicketResponse> ReadTicketResponsesOfTicket(int ticketNumber)
        {
            //In plaats van telkens tickets te casten, kunnen we op een ICollection ook de methode ToList oproepen die een List<> teruggeeft
            return tickets.ToList<Ticket>().Find(t => t.TicketNumber == ticketNumber).Responses;
        }

        public TicketResponse CreateTicketResponse(TicketResponse response)
        {
            //De nieuwe response krijgt als PK-waarde een auto-numbering toegewezen...
            //Dit door de hoogste ID van de ticketresponses op de zoeken en daar dan 1 bij op te tellen
            response.Id = responses.Max<TicketResponse>(r => r.Id) + 1;            
            responses.Add(response);
            return response;
        }

        public void UpdateTicketStateToClosed(int ticketnumber)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HardwareTicket> ReadHwTickets()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ticket> ReadNormalTickets()
        {
            throw new NotImplementedException();
        }
    }
}
