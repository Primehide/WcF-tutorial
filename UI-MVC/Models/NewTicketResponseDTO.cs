﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SC.UI.Web.MVC.Models
{
    public class NewTicketResponseDTO
    {
        public int TicketNumber { get; set; }
        public string ResponseText { get; set; }
        public bool IsClientResponse { get; set; }
    }
}