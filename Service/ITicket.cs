using SC.BL.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;

namespace Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.

    [ServiceKnownType(typeof(HardwareTicket))]
    //[ServiceKnownType(typeof(List<HardwareTicket>))]
    //[ServiceKnownType(typeof(List<Ticket>))]
    [ServiceKnownType(typeof(Collection<HardwareTicket>))]
    [ServiceKnownType(typeof(Collection<Ticket>))]
    [ServiceContract]
    public interface ITicket
    {
        [OperationContract]
        IEnumerable<Ticket> GetTickets();

        [OperationContract]
        IEnumerable<HardwareTicket> GetHwTickets();

        [OperationContract]
        Ticket GetTicket(int id);

        [OperationContract(Name = "CreateTicket")]
        Ticket CreateTicket(int accountId, string question);

        [OperationContract(Name = "CreateHwTicket")]
        Ticket CreateTicket(int accountId, string device, string problem);

        [OperationContract] 
        void CloseTicket(int ticketnumber);

        [OperationContract]
        TicketResponse AddTicketResponse(int ticketNumber, string response, bool isClientResponse);

        [OperationContract]
        IEnumerable<TicketResponse> GetTicketResponses(int ticketNumber);

        [OperationContract]
        string SayHello();
    }
}
