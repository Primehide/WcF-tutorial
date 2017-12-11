using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SC.UI.Web.MVC.Controllers.api
{
    public class TicketController : ApiController
    {
        private BL.ITicketManager mgr = new BL.TicketManager();

        [HttpPut]
        [Route("api/Ticket/{id}/State/Closed")]
        public IHttpActionResult PutTicketStateClosed(int id)
        {
            mgr.ChangeStateToClosed(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
