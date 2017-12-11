using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.ObjectModel;

namespace SC.BL.Domain
{
    [DataContract]
    //[KnownType(typeof(Ticket))]
    [KnownType(typeof(HardwareTicket))]
    [KnownType(typeof(TicketResponse))]
    [KnownType(typeof(Collection<TicketResponse>))]
    //[KnownType(typeof(List<HardwareTicket>))]
    //[KnownType(typeof(List<Ticket>))]
    [KnownType(typeof(Collection<HardwareTicket>))]
    //[KnownType(typeof(Collection<Ticket>))]
    [System.Xml.Serialization.XmlInclude(typeof(HardwareTicket))]
    public class Ticket
    {
        [DataMember]
        public int TicketNumber { get; set; }
        [DataMember]
        public int AccountId { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Er zijn maximaal 100 tekens toegestaan")]
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public DateTime DateOpened { get; set; }
        [DataMember]
        public TicketState State { get; set; }
        [DataMember]
        public ICollection<TicketResponse> Responses { get; set; }
    }
}
