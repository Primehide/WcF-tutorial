using SC.BL.Domain;
using System;
using System.Collections.Generic;
using SC.UI.CA.ExtensionMethods;
using SC.UI.CA.WcfReference;
using System.Security.Principal;

namespace SC.UI.CA
{
    class Program
    {
        private static bool quit = false;
        private static readonly BL.ITicketManager mgr = new BL.TicketManager();
        private static readonly Service svr = new Service();
        private static TicketClient wcfClient = new TicketClient("WSHttpBinding_ITicket");
        //private static TicketClient client = new TicketClient(); //basicHttpBinding
        //private static TCPReference.TicketClient tcpClient = new TCPReference.TicketClient(); //tcpBinding

        static void Main(string[] args)
        {
            while (!quit)
                ShowMenu();                
        }

        private static void CreateTestLogin()
        {
            System.ServiceModel.Description.ClientCredentials credentials = new System.ServiceModel.Description.ClientCredentials();
            credentials.UserName.UserName = "Test";
            credentials.UserName.Password = "test";

            wcfClient.ChannelFactory.Endpoint.EndpointBehaviors.RemoveAt(1);
            wcfClient.ChannelFactory.Endpoint.EndpointBehaviors.Add(credentials);
            Console.WriteLine(wcfClient.SayHello());
        }

        private static void CreatePrincipal()
        {
            WindowsIdentity MyIdentity = WindowsIdentity.GetCurrent();
            WindowsPrincipal MyPrincipal = new WindowsPrincipal(MyIdentity);
        }

        private static void ShowMenu()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("=== ISB204 HELPDESK - SUPPORT CENTER ===");
            Console.WriteLine("========================================");
            Console.WriteLine("1) Toon alle tickets");
            Console.WriteLine("2) Toon details van een ticket");
            Console.WriteLine("3) Toon de antwoorden van een ticket");
            Console.WriteLine("4) Maak een nieuw ticket");
            Console.WriteLine("5) Geef een antwoord op een ticket");
            Console.WriteLine("6) Markeer een ticket als gesloten");
            Console.WriteLine("0) Afsluiten");
            DetectMenuAction();
        }

        private static void DetectMenuAction()
        {
            bool inValidAction;
            do
            {
                inValidAction = false;
                Console.Write("Keuze: ");
                string input = Console.ReadLine();
                int action;
                if (Int32.TryParse(input, out action))
                {
                    switch (action)
                    {
                        case 0:
                            quit = true;
                            break;
                        case 1:
                            ShowAllTicketsMenuAction();
                            break;
                        case 2:
                            ReadOneTicketMenuAction();
                            break;
                        case 3:
                            ShowAllResponsesMenuAction();
                            break;
                        case 4:
                            CreateTicketMenuAction();
                            break;
                        case 5:
                            CreateResponseMenuAction();
                            break;
                        case 6:
                            ActionCloseTicket();
                            break;
                        default:
                            Console.WriteLine("Geen geldige keuze!");
                            inValidAction = true;
                            break;
                    }
                }
            } while (inValidAction);
        }

        private static void ActionCloseTicket()
        {
            Console.WriteLine("Ticketnummber: ");
            int ticketnummer = Int32.Parse(Console.ReadLine());

            //mgr.ChangeStateToClosed(ticketnummer);
            //client.CloseTicket(ticketnummer);
            //tcpClient.CloseTicket(ticketnummer);
            wcfClient.CloseTicket(ticketnummer);
        }

        private static void CreateResponseMenuAction()
        {
            Console.Write("Ticketnummer: ");
            int ticketNumber = Int32.Parse(Console.ReadLine());
            Console.Write("Antwoord: ");
            string response = Console.ReadLine();

            //mgr.AddTicketResponse(ticketNumber, response, false);
            //svr.AddTicketResponse(ticketNumber, response, false);
            wcfClient.AddTicketResponse(ticketNumber, response, false);
        }

        private static void ShowAllResponsesMenuAction()
        {
            bool validInput = false;
            int nbr = 0;
            while (!validInput)
            {
                Console.Write("Ticketnummer: ");
                string inputOfUser = Console.ReadLine();
                validInput = Int32.TryParse(inputOfUser, out nbr);
            }

            //IEnumerable<TicketResponse> responses = mgr.GetTicketResponses(nbr);
            IEnumerable<TicketResponse> responses = wcfClient.GetTicketResponses(nbr);
            foreach (var response in responses)
            {
                Console.WriteLine(response.GetInfo());
            }

        }

        private static void ReadOneTicketMenuAction()
        {
            bool validInput = false;
            int nbr = 0;
            while(!validInput)
            {
                Console.Write("Ticketnummer: ");
                string inputOfUser = Console.ReadLine();
                validInput = Int32.TryParse(inputOfUser, out nbr);
            }
            //Ticket t = mgr.GetTicket(nbr);
            Ticket t = wcfClient.GetTicket(nbr);
            PrintTicketDetails(t);
        }

        private static void PrintTicketDetails(Ticket ticket)
        {
            Console.WriteLine("{0,-15}: {1}", "Ticket", ticket.TicketNumber);
            Console.WriteLine("{0,-15}: {1}", "Gebruiker", ticket.AccountId);
            Console.WriteLine("{0,-15}: {1}", "Datum", ticket.DateOpened.ToString("dd/MM/yyyy"));
            Console.WriteLine("{0,-15}: {1}", "Status", ticket.State);

            if (ticket is HardwareTicket)
                Console.WriteLine("{0,-15}: {1}", "Toestel", ((HardwareTicket)ticket).DeviceName);

            Console.WriteLine("{0,-15}: {1}", "Vraag/probleem", ticket.Text);
        }


        private static void CreateTicketMenuAction()
        {
            string deviceName = null;
            Console.Write("Is het een HW-issue (j/n)? ");
            string hwResponse = Console.ReadLine();
            bool isHardwareProblem = (hwResponse.ToLower() == "j");
            if (isHardwareProblem)
            {
                Console.Write("Naam van het kapotte toestel: ");
                deviceName = Console.ReadLine();
            }

            Console.Write("Gebruikersnummer: ");
            int accountNumber = Int32.Parse(Console.ReadLine());
            Console.Write("Probleem: ");
            string problem = Console.ReadLine();

            //Naargelang het een HW-issue is of niet
            // moeten we een andere methode op de BL-laag
            // oproepen!
            if (!isHardwareProblem)
                //mgr.AddTicket(accountNumber, problem);
                wcfClient.CreateTicket(accountNumber, problem);
            else
                //mgr.AddTicket(accountNumber, deviceName, problem);
                wcfClient.CreateHwTicket(accountNumber, deviceName, problem);
        }

        private static void ShowAllTicketsMenuAction()
        {
            //IEnumerable<Ticket> allTickets = mgr.GetTickets();
            IEnumerable<Ticket> allTickets = wcfClient.GetTickets();


            foreach (var item in allTickets)
            {
                Console.WriteLine(item.GetInfo());
            }
        }
    }
}
