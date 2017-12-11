using SC.BL.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SC.UI.CA
{
    public class ProgramForValidationTesting
    {
        static void Main(string[] args)
        {
            Ticket t1 = new Ticket()
            {
                TicketNumber = 1,
                AccountId = 1,
                Text = "a",
                State = TicketState.Open,
                DateOpened = DateTime.Now
            };
            var errors = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(t1, new ValidationContext(t1), errors, validateAllProperties: true);

            if(!isValid)
            {
                foreach (var item in errors)
                    Console.WriteLine(item.ErrorMessage);
            }
            else
                Console.WriteLine("Ticket1 is valid");

            Ticket t2 = new HardwareTicket()
            {
                TicketNumber = 2,
                AccountId = 1,
                DeviceName = "PC-9876",
                Text = "text",
                State = TicketState.Open,
                DateOpened = DateTime.Now
            };
            var errors2 = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(t2, new ValidationContext(t2), errors2, validateAllProperties: true);
            if(!isValid)
            {
                foreach (var item in errors2)
                {
                    Console.WriteLine(item.ErrorMessage);
                }
            }
            else
                Console.WriteLine("Ticket2 is valid");

            TicketResponse tr = new TicketResponse()
            {
                Id = 1,
                Text = "response",
                IsClientResponse = true,
                Date = new DateTime(2014, 1, 1),
                Ticket = new Ticket()
                {
                    TicketNumber = 3,
                    AccountId = 1,
                    Text = "text",
                    State = TicketState.Open,
                    DateOpened = new DateTime(2015, 1, 1)
                }
            };

            var errors3 = new List<ValidationResult>();
             isValid = Validator.TryValidateObject(tr, new ValidationContext(tr), errors3, true);
            if(!isValid)
                foreach (var item in errors3) Console.WriteLine(item.ErrorMessage);
            else
                Console.WriteLine("TicketResponse OK");



            Console.ReadLine();
        }

       

    }

}
