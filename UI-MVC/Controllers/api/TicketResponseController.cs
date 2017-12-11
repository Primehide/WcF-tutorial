using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SC.UI.Web.MVC.Controllers.api
{
    public class TicketResponseController : ApiController
    {
        private BL.ITicketManager mgr = new BL.TicketManager();

        public IHttpActionResult Get(int id)
        {
            var responses = mgr.GetTicketResponses(id);
            if (responses.Count() == 0 || responses == null)
                return StatusCode(HttpStatusCode.NoContent);

            return Ok(responses);     
        }

        public Models.TicketResponseDTO Post(Models.NewTicketResponseDTO response)
        {
            BL.Domain.TicketResponse CreatedResponse = mgr.AddTicketResponse(response.TicketNumber, 
                                                                                response.ResponseText, 
                                                                                response.IsClientResponse);
            Models.TicketResponseDTO responseData = new Models.TicketResponseDTO()
            {
                Id = CreatedResponse.Id,
                Text = CreatedResponse.Text,
                Date = CreatedResponse.Date,
                IsClientResponse = CreatedResponse.IsClientResponse
            };

            return responseData;
        }
    }
}
