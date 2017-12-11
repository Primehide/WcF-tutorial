using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace SC.BL.Domain
{
    [DataContract]
    public class TicketResponse
    {
        [DataMember]
        public int Id { get; set; }
        [Required]
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public bool IsClientResponse { get; set; }

        //ignore nodig voor circulaire referentie te vermijden!!!!
        [IgnoreDataMember]
        public Ticket Ticket { get; set; }
    }
}
