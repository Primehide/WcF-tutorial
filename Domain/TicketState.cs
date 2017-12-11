using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC.BL.Domain
{
    public enum TicketState : byte
    {
        Open = 1,
        Answered,
        ClientAnswer,
        Closed
    }

}
