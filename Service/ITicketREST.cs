using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using SC.BL.Domain;
using Service.Models;
using System.Net.Http;

namespace Service
{
    [ServiceContract]
    public interface ITicketREST
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetTickets")]
        IEnumerable<Ticket> GetTicketsREST();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "CreateTicket")]
        HttpResponseMessage CreateTicketREST(TicketDTO ticket);
    }
}
